using UnityEngine;

public class HandScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // state
    public enum HandState { Default, Hold, MagicCharge, MagicReady, MagicRelease, MagicCollect, Hurt, Stunned, Block };
    [Header("state")]
    public HandState curHandState;
    [HideInInspector] public int handIndex;
    [HideInInspector] public float handDir;

    // fire charge
    [HideInInspector] public Transform fireChargeTransform;
    [HideInInspector] public GameObject fireChargeObject;
    [HideInInspector] public FireChargeScript myFireChargeScript;

    // magic collect effects
    Transform magicCollectTransform;
    ParticleSystem magicCollectParticleSystem;
    float magicCollectParticleEmissionRateOriginal;

    // just fired
    [HideInInspector] public int justFiredDur, justFiredCounter;

    // damage deal
    [HideInInspector] public bool createdDamageDeal;

    // equipment
    public Transform ringEquippedTransform;
    public Transform braceletEquippedTransform;
    public Transform weaponEquippedTransform;
    [HideInInspector] public GameObject ringEquippedObject;
    [HideInInspector] public GameObject braceletEquippedObject;
    [HideInInspector] public GameObject weaponEquippedObject;

    // robe
    [HideInInspector] public Transform myRobeTransform;
    [HideInInspector] public MeshRenderer myRobeClothMeshRenderer;

    // holding
    [HideInInspector] public ObjectScript objectHoldingScript;

    // offset
    Vector3 localPosSet;
    Vector3 localOffDefault;
    Vector3 localOffHold;
    Vector3 localOffMagicCharge;
    Vector3 localOffMagicReady;
    Vector3 localOffMagicRelease;
    Vector3 localOffMagicCollect;
    Vector3 localOffHurt;
    Vector3 localOffBlock;
    Vector3 localOffStunned;
    Vector3 localOffOriginal;
    Vector3 localOffCur;
    Vector3 attackOffCur;

    Vector3 localLookOffTarget;
    Vector3 localLookOffCur;

    Vector3 groundedOffTarget;
    Vector3 groundedOffCur;

    Vector3 moveOffTarget;
    Vector3 moveOffCur;

    Vector3 localKickOffTarget;
    Vector3 localKickOffCur;

    // rotation
    Vector3 localRotDefault;
    Vector3 localRotHold;
    Vector3 localRotMagicCharge;
    Vector3 localRotMagicReady;
    Vector3 localRotMagicRelease;
    Vector3 localRotMagicCollect;
    Vector3 localRotHurt;
    Vector3 localRotBlock;
    Vector3 localRotStunned;
    Vector3 localRotOriginal;
    Vector3 localRotCur;

    // magic mode
    public enum MagicMode { Out, In };
    public MagicMode curMagicMode;
    [HideInInspector] public int magicChargeIndex, magicChargeRate, magicChargeCounter;

    // melee attack
    int meleeAttackIndex;
    [HideInInspector] public int meleeAttackDur, meleeAttackCounter;
    [HideInInspector] public bool inMeleeAttack;
    int meleeAttackIndexResetDur, meleeAttackIndexResetCounter;

    // meshes
    [Header("meshes")]
    public MeshRenderer myHandMeshRenderer;
    public MeshFilter myHandMeshFilter;
    public Mesh defaultHandMesh;
    public Mesh holdHandMesh;
    public Mesh magicChargeHandMesh;
    public Mesh magicReadyHandMesh;
    public Mesh magicReleaseHandMesh;
    public Mesh magicCollectHandMesh;
    public Mesh hurtHandMesh;
    public Mesh stunnedHandMesh;
    public Mesh blockHandMesh;
    public Mesh collectHandMesh;
    int collectMeshFlickerIndex, collectMeshFlickerRate, collectMeshFlickerCounter;

    // init
    [HideInInspector] public bool initialized;

    public void Init ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // state
        handDir = (handIndex == 0 ) ? 1f : -1f;

        // attacks
        meleeAttackIndex = 1;
        meleeAttackIndexResetDur = 10;
        meleeAttackIndexResetCounter = meleeAttackIndexResetDur;

        // robe
        myRobeTransform = myTransform.Find("robe").transform;
        if ( myRobeTransform != null )
        {
            myRobeTransform.parent = null;
            myRobeClothMeshRenderer = ( handIndex == 0 ) ? myRobeTransform.Find("cloth").GetComponent<MeshRenderer>() : myRobeTransform.GetComponent<MeshRenderer>();
        }

        // offsets
        localOffOriginal = myTransform.localPosition;
        localOffCur = localOffOriginal;
        localOffDefault = localOffOriginal;
        localOffHold = localOffOriginal;
        localOffHold.y -= .025f;

        localOffMagicCharge = localOffOriginal;
        localOffMagicCharge.z += .05f;

        localOffMagicReady = localOffOriginal;
        localOffMagicReady.z += -.025f;

        localOffMagicRelease = localOffOriginal;
        localOffMagicRelease.z += .05f;

        localOffMagicCollect = localOffOriginal;
        localOffMagicCollect.z += .05f;

        localOffHurt = localOffOriginal;
        localOffHurt.z += .025f;
        localOffHurt.y += .0125f;

        localOffBlock = localOffOriginal;
        localOffBlock.z += .025f;
        localOffBlock.y += .0125f;

        localOffStunned = localOffOriginal;
        localOffStunned.z += .025f;
        localOffStunned.y += .0125f;

        // rotations
        localRotOriginal = myTransform.localRotation.eulerAngles;
        localRotCur = localRotOriginal;

        localRotDefault = localRotOriginal;

        localRotHold = localRotDefault;
        localRotHold.z += 90f;
        localRotHold.x -= 60f;

        localRotMagicCharge = localRotDefault;
        //localRotMagicCharge.x -= 20f;
        //localRotMagicCharge.z -= (35f * handDir);

        localRotMagicReady = localRotDefault;
        //localRotMagicReady.x -= 20f;
        localRotMagicReady.z -= (20f * handDir);

        localRotMagicRelease = localRotDefault;
        localRotMagicRelease.x -= 20f;
        localRotMagicRelease.z += (35f * handDir);

        localRotMagicCollect = localRotDefault;
        localRotMagicCollect.x -= 20f;
        localRotMagicCollect.z += (35f * handDir);

        localRotHurt = localRotDefault;
        localRotHurt.x += 20f;

        localRotBlock = localRotDefault;
        localRotBlock.x += 20f;

        localRotStunned = localRotDefault;
        localRotStunned.x += 20f;

        // collect mesh flicker
        collectMeshFlickerIndex = 0;
        collectMeshFlickerRate = 8;
        collectMeshFlickerCounter = 0;

        // magic charge
        magicChargeIndex = 0;
        magicChargeRate = 8;
        magicChargeCounter = 0;

        // fire charge object
        fireChargeObject = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.fireChargePrefab,myTransform.position,Quaternion.identity,1f);
        fireChargeTransform = fireChargeObject.transform;
        fireChargeTransform.parent = myTransform;
        myFireChargeScript = fireChargeObject.GetComponent<FireChargeScript>();
        myFireChargeScript.scaleFacMultiplier = 1f;

        // magic collect effects
        GameObject magicCollectO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.magicCollectPrefab,myTransform.position,Quaternion.identity,1f);
        magicCollectTransform = magicCollectO.transform;
        magicCollectTransform.parent = myTransform;
        BasicFunctions.ResetTransform(magicCollectTransform);
        float scl = .025f;
        magicCollectTransform.localScale = Vector3.one * scl;
        magicCollectParticleSystem = magicCollectO.GetComponent<ParticleSystem>();
        magicCollectParticleEmissionRateOriginal = magicCollectParticleSystem.emissionRate;

        // counters
        justFiredDur = 8;
        justFiredCounter = justFiredDur;

        // damage deal
        createdDamageDeal = false;

        // done
        initialized = true;
    }

    void GetOffset ()
    {
        float mouseFac = .25f;
        float mouseInputH = (InputManager.instance.lookDirection.x * mouseFac) * Time.deltaTime;
        float mouseInputV = (InputManager.instance.lookDirection.y * mouseFac) * Time.deltaTime;
        float moveInputH = InputManager.instance.moveDirection.x;
        float moveInputV = InputManager.instance.moveDirection.y;

        if ( SetupManager.instance.inFreeze || SetupManager.instance.defeatedFinalBoss )
        {
            mouseInputH = 0f;
            mouseInputV = 0f;
            moveInputH = 0f;
            moveInputV = 0f;
        }

        float offsetThreshold = 0f;
        float offMax = .025f;

        bool movingMouse = (Mathf.Abs(mouseInputH) > offsetThreshold || Mathf.Abs(mouseInputV) > offsetThreshold);
        if (movingMouse)
        {
            localLookOffTarget += new Vector3(mouseInputH,mouseInputV,0f);
        }
        if ( !movingMouse )
        {
            localLookOffTarget = Vector3.Lerp(localLookOffTarget,Vector3.zero,10f * Time.deltaTime);
        }

        // player movement
        float movingThreshold = .1f;
        bool moving = (Mathf.Abs(moveInputH) > movingThreshold || Mathf.Abs(moveInputV) > movingThreshold);
        if (moving && HandManager.instance.drifterScript.controller.isGrounded && !GameManager.instance.playerFirstPersonDrifter.playerBlocking && !GameManager.instance.playerFirstPersonDrifter.inKick)
        {
            float t0 = Time.time * 12.5f;
            float f0 = .01f;
            float s0 = Mathf.Sin(t0) * (f0 * handDir);
            moveOffTarget.y = s0;
        }
        else
        {
            if (!SetupManager.instance.inFreeze)
            {
                moveOffTarget = Vector3.Lerp(moveOffTarget, Vector3.zero, 10f * Time.deltaTime);
            }
        }

        // player grounded
        if ( !HandManager.instance.drifterScript.controller.isGrounded )
        {
            groundedOffTarget.x += (.125f * -handDir);
            groundedOffTarget.y -= .075f;
        }

        if (!SetupManager.instance.inFreeze)
        {
            groundedOffTarget = Vector3.Lerp(groundedOffTarget, Vector3.zero, 10f * Time.deltaTime);
        }

        // clampie
        localLookOffTarget.x = Mathf.Clamp(localLookOffTarget.x,-offMax,offMax);
        localLookOffTarget.y = Mathf.Clamp(localLookOffTarget.y,-offMax,offMax);
        localLookOffTarget.z = Mathf.Clamp(localLookOffTarget.z,-offMax,offMax);

        float xKickAdd = (-.025f * handDir);
        float zKickAdd = -.0125f;
        if (GameManager.instance.playerFirstPersonDrifter.inKick)
        {
            localKickOffTarget.x = xKickAdd;
            localKickOffTarget.y = .0375f;
            localKickOffTarget.z = zKickAdd;
        }
        else
        {
            if (!SetupManager.instance.inFreeze)
            {
                localKickOffTarget = Vector3.Lerp(localKickOffTarget, Vector3.zero, 30f * Time.deltaTime);
            }
        }

        groundedOffTarget.x = Mathf.Clamp(groundedOffTarget.x, -offMax, offMax);
        groundedOffTarget.y = Mathf.Clamp(groundedOffTarget.y, -offMax, offMax);
        groundedOffTarget.z = Mathf.Clamp(groundedOffTarget.z, -offMax, offMax);

        moveOffTarget.x = Mathf.Clamp(moveOffTarget.x, -offMax, offMax);
        moveOffTarget.y = Mathf.Clamp(moveOffTarget.y, -offMax, offMax);
        moveOffTarget.z = Mathf.Clamp(moveOffTarget.z, -offMax, offMax);

        // lerpie
        if (!SetupManager.instance.inFreeze)
        {
            localLookOffCur = Vector3.Lerp(localLookOffCur, localLookOffTarget, 10f * Time.deltaTime);
            groundedOffCur = Vector3.Lerp(groundedOffCur, groundedOffTarget, 20f * Time.deltaTime);
            localKickOffCur = Vector3.Lerp(localKickOffCur, localKickOffTarget, 30f * Time.deltaTime);
            moveOffCur = Vector3.Lerp(moveOffCur, moveOffTarget, 20f * Time.deltaTime);
        }
    }

    void LateUpdate ()
    {
        if (!SetupManager.instance.paused /*&& (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur)*/ )
        {
            //if (!GameManager.instance.inFreeze)
            {
                GetOffset();

                localPosSet = localOffCur;

                // just fired?
                if (justFiredCounter < justFiredDur)
                {
                    justFiredCounter++;
                }

                // position
                attackOffCur = Vector3.zero;

                Vector3 rotExtraOff = Vector3.zero;

                // object holding rotation offset?
                if (objectHoldingScript != null)
                {
                    objectHoldingScript.rotOffset = Vector3.zero;

                    // object holding hurt state
                    if (curHandState == HandState.Hurt)
                    {
                        //objectHoldingScript.rotOffset.y += 90f;
                        objectHoldingScript.rotOffset.z += 90f;
                        objectHoldingScript.rotOffset.x += 45f;
                    }

                    // object holding stun state
                    if (curHandState == HandState.Stunned)
                    {
                        //objectHoldingScript.rotOffset.y += 90f;
                        objectHoldingScript.rotOffset.z += 90f;
                        objectHoldingScript.rotOffset.x += 45f;
                    }
                }

                // handle melee attack
                if (inMeleeAttack)
                {
                    if (meleeAttackCounter < meleeAttackDur)
                    {
                        if (!GameManager.instance.playerHurt && !GameManager.instance.playerFirstPersonDrifter.playerStunned && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur))
                        {
                            meleeAttackCounter++;

                            // created damage deal
                            if (!createdDamageDeal && (meleeAttackCounter >= (meleeAttackDur / 2)))
                            {
                                Vector3 damageDealP = myTransform.position;
                                damageDealP += HandManager.instance.myTransform.forward * .25f;
                                PrefabManager.instance.SpawnDamageDeal(damageDealP, 2.5f, 1, Npc.AttackData.DamageType.Melee, 10, HandManager.instance.myTransform, 1f, true, DamageDeal.Target.AI, null, false, false);
                                createdDamageDeal = true;

                                // also shoot fire with melee attack?
                                if ((SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.FireRage) && SetupManager.instance.runDataRead.playerHealthCur <= 1) || (SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.ShootFire)))
                                {
                                    CastMagic(1, false, true);
                                }
                            }
                        }

                        // attack animation
                        float p0 = BasicFunctions.ConvertRange((float)meleeAttackCounter, 0f, (float)meleeAttackDur * 1f, 0f, 1f);

                        float sideOff0 = HandManager.instance.meleeAttackSwingCurveSide.Evaluate(p0) * (.575f * handDir);
                        float forwardOff0 = HandManager.instance.meleeAttackSwingCurveForward.Evaluate(p0) * .375f;
                        float upOff0 = HandManager.instance.meleeAttackSwingCurve.Evaluate(p0) * .15f;
                        float r0 = HandManager.instance.meleeAttackSwingCurveSide.Evaluate(p0);

                        switch (meleeAttackIndex)
                        {
                            case 0:
                                attackOffCur.x = sideOff0;
                                attackOffCur.y = upOff0;
                                attackOffCur.z = forwardOff0;
                                attackOffCur.y += .025f;

                                rotExtraOff.y = r0 * 300f;
                                break;
                            case 1:
                                attackOffCur.x = -sideOff0;
                                attackOffCur.y = upOff0;
                                attackOffCur.z = forwardOff0;
                                attackOffCur.x += (.075f * handDir);
                                attackOffCur.z += .025f;
                                attackOffCur.y -= .025f;

                                rotExtraOff.y = r0 * 300f;
                                rotExtraOff.z = -45f + (180f * r0);
                                break;
                        }
                    }
                    else
                    {
                        createdDamageDeal = false;
                        inMeleeAttack = false;
                        attackOffCur = Vector3.zero;
                        meleeAttackIndexResetCounter = 0;
                    }
                }
                else
                {
                    if (meleeAttackIndexResetCounter < meleeAttackIndexResetDur)
                    {
                        meleeAttackIndexResetCounter++;
                    }
                    else
                    {
                        meleeAttackIndex = 1;
                    }
                }

                localPosSet += attackOffCur;

                //float xKickAdd = (-.0125f * handDir);
                //float zKickAdd = -.005f;
                //if ( GameManager.instance.playerFirstPersonDrifter.inKick )
                //{
                //    localPosSet.x += xKickAdd;
                //    localPosSet.z += zKickAdd;
                //}

                Vector3 p = localPosSet;
                p.x += localLookOffCur.x;
                p.y += localLookOffCur.y;
                p.z += localLookOffCur.z;

                p.x += groundedOffCur.x;
                p.y += groundedOffCur.y;
                p.z += groundedOffCur.z;

                p.x += moveOffCur.x;
                p.y += moveOffCur.y;
                p.z += moveOffCur.z;

                p.x += localKickOffCur.x;
                p.y += localKickOffCur.y;
                p.z += localKickOffCur.z;

                myTransform.localPosition = p;//localPosSet + localLookOffCur + groundedOffCur + moveOffCur;

                // rotation
                myTransform.localRotation = Quaternion.Euler(localRotCur) * Quaternion.Euler(rotExtraOff);

                // update robe?
                if (myRobeTransform != null)
                {
                    Vector3 robeP = myTransform.position;
                    myRobeTransform.position = robeP;

                    Vector3 r0 = myRobeTransform.position;
                    Vector3 r1 = HandManager.instance.myTransform.position;
                    r1 += (HandManager.instance.myTransform.right * (-.1f * handDir));
                    r1 += (HandManager.instance.myTransform.forward * -.1f);
                    r1 += (HandManager.instance.myTransform.up * -.175f);

                    Vector3 r2 = (r1 - r0).normalized;
                    Quaternion targetRot = Quaternion.LookRotation(r2, HandManager.instance.myTransform.up) * Quaternion.Euler(-90f, 0f, 0f);
                    myRobeTransform.rotation = targetRot;
                }

                // update fire charge position?
                if (myFireChargeScript != null && myFireChargeScript.myTransform != null)
                {
                    Vector3 fireChargeP = Vector3.zero;
                    fireChargeP.y -= 1.25f;
                    fireChargeP.z -= 1.25f;
                    //fireChargeP.y += .375f;
                    myFireChargeScript.myTransform.localPosition = fireChargeP;

                    // magic collect particles?
                    Vector3 magicCollectP = fireChargeP;
                    magicCollectP.z += .5f;
                    //magicCollectP.z += .5f;
                    bool showMagicCollectParticles = (curHandState == HandState.MagicCollect);
                    magicCollectParticleSystem.emissionRate = (showMagicCollectParticles) ? magicCollectParticleEmissionRateOriginal : 0f;
                    magicCollectTransform.localPosition = magicCollectP;
                }
            }

            // update equipment?
            UpdateEquipment();
        }
    }

    public void UpdateEquipment ()
    {
        // ring
        if ( ringEquippedTransform != null )
        {
            Vector3 ringP = myTransform.position;

            switch ( curHandState )
            {
                default:
                    ringP += (myTransform.forward * -.04f);
                    ringP += (myTransform.right * -.01325f);
                    ringP += (myTransform.up * -.00375f);
                    break;
            }

            Vector3 r0 = myTransform.position;
            Vector3 r1 = ringP;
            Vector3 r2 = (r1 - r0).normalized;
            Vector3 up = myTransform.up;
            Quaternion ringR = Quaternion.LookRotation(r2,up) * Quaternion.Euler(90f, 0f, 0f);

            ringEquippedTransform.position = ringP;
            ringEquippedTransform.rotation = ringR;
        }

        // bracelet
        if ( braceletEquippedTransform != null )
        {
            Vector3 braceletP = myTransform.position;
            braceletP += (myRobeTransform.up * -.005f);

            Vector3 r0 = myRobeTransform.position;
            r0 += myRobeTransform.up * .05f;
            Vector3 r1 = myTransform.position;
            Vector3 r2 = (r1 - r0).normalized;
            Vector3 up = myTransform.up;
            Quaternion braceletR = Quaternion.LookRotation(r2,up) * Quaternion.Euler(-90f,0f,0f);

            braceletEquippedTransform.position = braceletP;
            braceletEquippedTransform.rotation = braceletR;
        }
    }

    public void SetHandState ( HandState _to )
    {
        if (initialized)
        {
            bool hideFireCharge = true;

            Vector3 localOffSetTo = localOffCur;
            Vector3 localRotSetTo = localRotCur;
            switch (_to)
            {
                case HandState.Default:
                    if (curMagicMode == MagicMode.Out)
                    {
                        myHandMeshFilter.mesh = defaultHandMesh;
                        localRotSetTo = localRotDefault;
                        localOffSetTo = localOffDefault;
                    }
                    else if ( curMagicMode == MagicMode.In )
                    {
                        myHandMeshFilter.mesh = defaultHandMesh;
                        localRotSetTo = localRotDefault;
                        localOffSetTo = localOffDefault;
                        localRotSetTo.y += 180f;
                        localRotSetTo.x += 80f;
                        localRotSetTo.z += 60f;
                    }
                    break;
                case HandState.Hold: myHandMeshFilter.mesh = holdHandMesh; localRotSetTo = localRotHold; localOffSetTo = localOffHold; break;
                case HandState.MagicRelease:
                    myHandMeshFilter.mesh = magicReleaseHandMesh;
                    localRotSetTo = localRotMagicRelease;
                    localOffSetTo = localOffMagicRelease;
                break;
                case HandState.MagicCharge:
                    myHandMeshFilter.mesh = magicChargeHandMesh;
                    localRotSetTo = localRotMagicCharge;
                    localOffSetTo = localOffMagicCharge;

                    // audio
                    if (curHandState != _to)
                    {
                        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.magicChargeClips), .5f, .7f, .15f, .175f);
                    }
                        break;
                case HandState.MagicReady:
                    myHandMeshFilter.mesh = magicReadyHandMesh;
                    localRotSetTo = localRotMagicReady;
                    localOffSetTo = localOffMagicReady;

                    // audio
                    if (curHandState != _to)
                    {
                        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireStartClips), 1.4f, 1.7f, .15f, .175f);
                    }

                    hideFireCharge = false;
                    break;
                case HandState.MagicCollect:

                    if ( collectMeshFlickerCounter < collectMeshFlickerRate )
                    {
                        collectMeshFlickerCounter++;
                    }
                    else
                    {
                        collectMeshFlickerIndex = (collectMeshFlickerIndex == 0) ? 1 : 0;
                        collectMeshFlickerCounter = 0;
                    }
                    Mesh meshUse = (collectMeshFlickerIndex == 0) ? magicCollectHandMesh : magicReadyHandMesh;
                    myHandMeshFilter.mesh = meshUse;
                    //localRotSetTo = localRotMagicCollect;
                    //localOffSetTo = localOffMagicCollect;

                    localRotSetTo = localRotDefault;
                    localRotSetTo.y += 180f;
                    localRotSetTo.x += 80f;
                    localRotSetTo.z += 60f;

                    localOffSetTo = localOffMagicRelease;
                    float t0 = Time.time * 20f;
                    float f0 = .0075f;
                    float s0 = Mathf.Sin(t0) * f0;
                    float s1 = Mathf.Cos(t0) * f0;
                    localOffSetTo.x += s0;
                    localOffSetTo.y += s1;
                    localOffSetTo.z -= .05f;
                    break;

                case HandState.Hurt:
                    myHandMeshFilter.mesh = hurtHandMesh;
                    localRotSetTo = localRotHurt;
                    localOffSetTo = localOffHurt;

                    localOffSetTo.x += (.025f * handDir);
                    localOffSetTo.y -= (.025f * handDir);
                    localOffSetTo.z -= .025f;
                break;

                case HandState.Block:
                    myHandMeshFilter.mesh = blockHandMesh;
                    localRotSetTo = localRotBlock;
                    localOffSetTo = localOffBlock;

                    localOffSetTo.x += (.025f * handDir);
                    localOffSetTo.y -= (.025f * handDir);
                    localOffSetTo.z -= .025f;
                    break;

                case HandState.Stunned:
                    myHandMeshFilter.mesh = stunnedHandMesh;
                    localRotSetTo = localRotStunned;
                    localOffSetTo = localOffStunned;

                    localOffSetTo.x += (.025f * handDir);
                    localOffSetTo.y -= (.025f * handDir);
                    localOffSetTo.z -= .025f;
                    break;
            }
            localOffCur = localOffSetTo;
            localRotCur = localRotSetTo;

            // show or hide fire charge
            if (myFireChargeScript != null)
            {
                myFireChargeScript.myMeshRenderer.enabled = (!hideFireCharge);
                myFireChargeScript.empty = (SetupManager.instance.runDataRead.playerFireCur <= 0);
            }

            curHandState = _to;

            // log
            //Debug.Log("set hand state to: " + _to + " || " + Time.time.ToString());
        }
    }

    public void Charge ()
    {
        switch ( magicChargeIndex )
        {
            case 0: magicChargeRate = 8; break;
            case 1: magicChargeRate = 24; break;
            case 2: magicChargeRate = 16; break;
        }

        int magicChargeIndexMax = 2;
        if (magicChargeIndex < magicChargeIndexMax)
        {
            if (magicChargeCounter < magicChargeRate)
            {
                magicChargeCounter++;
            }
            else
            {
                bool preventNextChargeStage = false;
                //if ( magicChargeIndex == 1 && GameManager.instance.playerFireCount >= SetupManager.instance.curProgressData.playerFireMax)
                //{
                //    preventNextChargeStage = true;
                //}
                if (!preventNextChargeStage)
                {
                    magicChargeIndex++;
                    magicChargeCounter = 0;
                }
                else
                {
                    magicChargeIndex = 1;
                    magicChargeCounter = 0;
                }

                //switch ( magicChargeIndex )
                //{
                //    case 1: AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.magicReadyClips),.6f,.9f,.3f,.325f); break;
                //}

                //if ( GameManager.instance.playerFireCount <= 0 )
                //{
                //    StopCharge();
                //    CastMagic();
                //}
            }
        }

        // log
        //Debug.Log("charge: " + magicChargeCounter.ToString() + "/" + magicChargeRate.ToString() + " || " + magicChargeIndex.ToString() + " || " + Time.time.ToString());
    }

    public void StopCharge ()
    {
        magicChargeIndex = 0;
        magicChargeCounter = 0;

        // log
        //Debug.Log("stop charge! || " + Time.time.ToString());
    }

    public void CastMagic ( int _armIndex, bool _setHandRelease, bool _ignoreFireCount )
    {
        float armDir = (_armIndex == 0) ? 1f : -1f;

        if (_setHandRelease)
        {
            SetHandState(HandState.MagicRelease);
        }

        Transform camTransform = Camera.main.transform;
        Vector3 projectileSpawnPos = myTransform.position;
        projectileSpawnPos += camTransform.right * -(.05f * armDir);
        projectileSpawnPos += camTransform.up * .0175f;
        projectileSpawnPos += camTransform.forward * .175f;

        Quaternion projectileSpawnRot = Quaternion.LookRotation(camTransform.forward);

        if (SetupManager.instance.runDataRead.playerFireCur > 0 || _ignoreFireCount)
        {
            if (SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Spray))
            {
                for (int i = 0; i < 2; i++)
                {
                    float fireDir = (i == 0) ? 1f : -1f;
                    GameObject projectileO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.fireBallPrefab, projectileSpawnPos, projectileSpawnRot, .125f);
                    ProjectileScript projectileScript = projectileO.GetComponent<ProjectileScript>();
                    projectileScript.dir = camTransform.forward;
                    projectileScript.dir += (camTransform.right * (.175f * fireDir));
                    projectileScript.speed = 12f;
                    projectileScript.radius = .25f;
                    projectileScript.SetOwnerType(ProjectileScript.OwnerType.Player);
                }
            }
            else
            {
                GameObject projectileO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.fireBallPrefab, projectileSpawnPos, projectileSpawnRot, .125f);
                ProjectileScript projectileScript = projectileO.GetComponent<ProjectileScript>();
                projectileScript.dir = camTransform.forward;
                projectileScript.speed = 12f;
                projectileScript.radius = .25f;
                projectileScript.SetOwnerType(ProjectileScript.OwnerType.Player);
            }
            if (!_ignoreFireCount)
            {
                GameManager.instance.AddPlayerFire(-1);

                // knockback from items?
                if (Mathf.Abs(SetupManager.instance.playerEquipmentStatsTotal.knockbackAdd) > .025f)
                {
                    GameManager.instance.playerFirstPersonDrifter.myImpactReceiver.AddImpact(GameManager.instance.playerFirstPersonDrifter.myTransform.forward,-SetupManager.instance.playerEquipmentStatsTotal.knockbackAdd);
                }
            }
        }
        else
        {
            Vector3 noFireLeftP = projectileSpawnPos;
            Quaternion noFireLeftR = Quaternion.LookRotation(GameManager.instance.mainCameraTransform.forward);
            GameObject noFireLeftO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.magicImpactParticlesLocalPrefab[0], noFireLeftP, noFireLeftR, .25f);
            Transform noFireLeftTr = noFireLeftO.transform;
            noFireLeftTr.parent = GameManager.instance.mainCameraTransform;

            GameManager.instance.SetPlayerFireFlicker(3);
        }

        if (_setHandRelease)
        {
            HandManager.instance.magicFireCounter = 0;
            justFiredCounter = 0;
        }

        // cast magic audio
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireMagicCastClips), 1.1f, 1.4f, .3f, .325f);
    }

    public void GrabObject ( ObjectScript _objectScript )
    {
        if (_objectScript != null)
        {
            _objectScript.Grab(this);
            objectHoldingScript = _objectScript;
        }
    }

    public void ReleaseObject (bool _clear)
    {
        if (objectHoldingScript != null)
        {
            if (_clear)
            {
                objectHoldingScript.Clear();
            }
            else
            {
                objectHoldingScript.Release();
            }

            objectHoldingScript = null;
        }
    }

    public void InitMeleeAttack ()
    {
        meleeAttackIndex = (meleeAttackIndex == 0) ? 1 : 0;
        meleeAttackDur = 24;
        meleeAttackDur = Mathf.RoundToInt(meleeAttackDur * (1f + SetupManager.instance.playerEquipmentStatsTotal.attackSpeedAdd));

        // swift strikes blessing?
        int meleeAttackDurMin = 12;
        if ( SetupManager.instance != null && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.SwiftStrikes) )
        {
            meleeAttackDur -= BlessingDatabase.instance.swiftStrikesFrameReduce;
        }
        if ( meleeAttackDur < meleeAttackDurMin )
        {
            meleeAttackDur = meleeAttackDurMin;
        }

        // done
        meleeAttackCounter = 0;
        inMeleeAttack = true;
        createdDamageDeal = false;
    }

    public void StopMeleeAttack ()
    {
        meleeAttackIndex = 0;
        meleeAttackCounter = 0;
        inMeleeAttack = false;
        createdDamageDeal = false;
    }

    public void SetMagicMode ( MagicMode _to )
    {
        curMagicMode = _to;

        // log
        //Debug.Log("set magic mode to: " + _to + " || " + Time.time.ToString());
    }

    public void UpdateSleeveMaterial ()
    {
        int bodyEquipmentIndex = SetupManager.instance.runDataRead.curBodyEquipmentIndex;
        if ( bodyEquipmentIndex != -1 )
        {
            EquipmentDatabase.Equipment bodyEquipment = (EquipmentDatabase.Equipment)bodyEquipmentIndex;
            EquipmentDatabase.EquipmentData bodyEquipmentData = EquipmentDatabase.instance.GetEquipmentData(bodyEquipment);
            myRobeClothMeshRenderer.material = bodyEquipmentData.matB;
        }
    }
}
