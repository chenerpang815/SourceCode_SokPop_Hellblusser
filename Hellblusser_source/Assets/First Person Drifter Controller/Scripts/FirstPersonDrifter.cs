// original by Eric Haines (Eric5h5)
// adapted by @torahhorse
// http://wiki.unity3d.com/index.php/FPSWalkerEnhanced

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class FirstPersonDrifter: MonoBehaviour
{
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;

    // lava
    [HideInInspector] public bool standingOnLava;
    [HideInInspector] public int hitLavaDur, hitLavaCounter;

    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
    private bool limitDiagonalSpeed = true;
 
    public bool enableRunning = false;
 
    public float jumpSpeed = 4.0f;
    public float gravity = 10.0f;

    // kick
    [HideInInspector] public bool inKick;
    [HideInInspector] public int kickDur, kickCounter;
    [HideInInspector] public bool createdKickDamageDeal;
    float kickPrepareFac, kickDoFac;
    public GameObject[] kickObjects;
    public Transform kickTransform;
    Vector3 kickPosIdle, kickPosPrepare, kickPosDo, kickPosTarget, kickPosCur;

    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    private float fallingDamageThreshold = 10.0f;
 
    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;
 
    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;
 
    public float slideSpeed = 5.0f;
 
    // If checked, then the player can change direction while in the air
    public bool airControl = true;
 
    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;
 
    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    public int antiBunnyHopFactor = 1;
 
    private Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public bool grounded = false;
    bool landed;
    [HideInInspector] public CharacterController controller;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private int jumpTimer;

    // blocking
    [HideInInspector] public bool playerBlocking;

    // stunned
    [HideInInspector] public int stunnedDur, stunnedCounter;
    [HideInInspector] public bool playerStunned;

    // input
    [HideInInspector] public float jumpFloat;

    // impact receiver
    [Header("impact receiver")]
    public ImpactReceiver myImpactReceiver;

    // audio
    //[Header("audio")]
    //public AudioSource footstepAudioSource;
    int footstepRate, footstepCounter;

    void Start ()
    {
        controller = GetComponent<CharacterController>();

        myTransform = transform;
        myGameObject = gameObject;

        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = 0;//antiBunnyHopFactor;

        footstepRate = 14;
        footstepCounter = 0;

        hitLavaDur = 30;
        hitLavaCounter = hitLavaDur;

        kickDur = 16;
        kickCounter = kickDur;
        kickPrepareFac = .325f;
        kickDoFac = .375f;
        inKick = false;
        HideKickObject();

        kickPosIdle = kickTransform.localPosition;
        kickPosPrepare = kickPosIdle;
        kickPosPrepare.z += .0325f;
        kickPosDo = kickPosPrepare;
        kickPosDo.z += .0325f;
        kickPosTarget = kickPosIdle;
        kickPosCur = kickPosTarget;

        stunnedDur = 10;
        stunnedCounter = stunnedDur;
        playerStunned = false;
    }

    void Update()
    {
        float inputX = InputManager.instance.moveDirection.x;
        float inputY = InputManager.instance.moveDirection.y;                                                     
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

        // player stunned?
        //Debug.Log("player stunned: " + playerStunned + " || " + stunnedCounter.ToString() + "/" + stunnedDur.ToString() + " || " + Time.time.ToString());
        if (stunnedCounter < stunnedDur)
        {
            if (!SetupManager.instance.inFreeze)
            {
                stunnedCounter++;
            }
            inputX = 0f;
            inputY = 0f;
            playerBlocking = false;
        }
        playerStunned = (stunnedCounter < stunnedDur);

        if (!SetupManager.instance.inFreeze && !SetupManager.instance.defeatedFinalBoss )
        {
            // player blocking
            if (!playerBlocking && !inKick)
            {
                bool allowedToBlock = true;
                if (SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.CantBlock) )
                {
                    allowedToBlock = false;
                }
                if (allowedToBlock && (!HandManager.instance.handScripts[1].inMeleeAttack && (InputManager.instance.keyboardBlock || InputManager.instance.gamepadBlock)))
                {
                    AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.footstepDefaultClips), .6f, .8f, .225f, .25f);
                    playerBlocking = true;
                }
            }
            else
            {
                if (!(InputManager.instance.keyboardBlock || InputManager.instance.gamepadBlock) || HandManager.instance.handScripts[1].inMeleeAttack)
                {
                    playerBlocking = false;
                }
            }
            if ( inKick )
            {
                playerBlocking = false;
            }
            if ( playerBlocking )
            {
                inputX = 0f;
                inputY = 0f;
            }

            // kick animation?
            if ( kickTransform != null )
            {
                float kickLerpie = 20f;

                kickPosTarget = kickPosIdle;
                if (inKick)
                {
                    if (kickCounter < (kickDur * kickPrepareFac))
                    {
                        kickPosTarget = kickPosPrepare;
                    }
                    else
                    {
                        kickPosTarget = kickPosDo;
                    }
                }

                kickPosCur = Vector3.Lerp(kickPosCur,kickPosTarget,kickLerpie * Time.deltaTime);
                kickTransform.localPosition = kickPosCur;
            }

            //Debug.Log("playerBlocking: " + playerBlocking.ToString() + " || " + Time.time.ToString());

            if (LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.lavaObject != null && LevelGeneratorManager.instance.lavaObject.activeSelf)
            {
                if (hitLavaCounter >= hitLavaDur)
                {
                    float cDst = 1f;
                    Vector3 c0 = myTransform.position;
                    c0.y += cDst;
                    Vector3 c1 = myTransform.position;
                    c1.y -= cDst;
                    RaycastHit cHit;
                    if (Physics.Linecast(c0, c1, out cHit, SetupManager.instance.lavaLayerMask))
                    {
                        Transform cHitTr = cHit.transform;
                        if (cHitTr != null)
                        {
                            GameObject cHitO = cHitTr.gameObject; ;
                            if (cHitO != null && cHitO.layer == 15)
                            {
                                hitLavaCounter = 0;
                                Jump();
                            }
                        }
                    }
                }
                if (hitLavaCounter < hitLavaDur)
                {
                    hitLavaCounter++;
                }
            }

            if (grounded)
            {
                if (!landed)
                {
                    if ( !standingOnLava )
                    {
                        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.footstepDefaultClips), .2f, .3f, .225f, .25f);
                    }
                    landed = true;
                }

                bool sliding = false;
                // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
                // because that interferes with step climbing amongst other annoyances
                if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
                {
                    if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                        sliding = true;
                }
                // However, just raycasting straight down from the center can fail when on steep slopes
                // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
                else
                {
                    Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                    if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                        sliding = true;
                }

                // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
                if (falling)
                {
                    falling = false;
                    if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                        FallingDamageAlert(fallStartLevel - myTransform.position.y);
                }

                // define movement speed
                speed = walkSpeed;
                if ( SetupManager.instance != null && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.QuickFeet) )
                {
                    speed += 1f;
                }
                speed += .25f;
                speed += SetupManager.instance.playerEquipmentStatsTotal.moveSpeedAdd;
                speed = Mathf.Clamp(speed,.1f,10f);

                // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
                if ((sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide"))
                {
                    Vector3 hitNormal = hit.normal;
                    moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                    Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                    moveDirection *= slideSpeed;
                    playerControl = false;
                }
                // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
                else
                {
                    moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                    moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                    playerControl = true;
                }

                // log
                //Debug.Log("jumpTimer: " + jumpTimer.ToString() + " || anti: " + antiBunnyHopFactor.ToString() + " || " + Time.time.ToString());

                // kick?
                if (kickCounter < kickDur)
                {
                    ShowKickObject();
                    kickCounter++;

                    if ( !createdKickDamageDeal && (kickCounter >= (kickDur * kickDoFac)) )
                    {
                        // force
                        Vector3 kickDir = myTransform.forward;
                        float kickForce = 20f;
                        myImpactReceiver.AddKickImpact(kickDir, kickForce);

                        // damage deal
                        Vector3 kickP = myTransform.position;
                        kickP += (myTransform.forward * .75f);
                        kickP += (myTransform.up * .25f);
                        PrefabManager.instance.SpawnDamageDeal(kickP,2.5f,1,Npc.AttackData.DamageType.Melee,10,myTransform,1f,true,DamageDeal.Target.AI,null,false,true);

                        // done
                        createdKickDamageDeal = true;
                    }
                }
                else
                {
                    if (inKick)
                    {
                        HideKickObject();
                        inKick = false;
                    }
                }
                if (GameManager.instance != null && !GameManager.instance.inInteraction && !inKick)
                {
                    if (InputManager.instance.kickPressed)
                    {
                        Kick();
                    }
                }

                // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
                if (GameManager.instance != null && !GameManager.instance.inInteraction && !inKick )
                {
                    if (InputManager.instance.jumpPressed || hitLavaCounter < hitLavaDur)
                    {
                        jumpTimer++;
                    }
                    else if (jumpTimer >= antiBunnyHopFactor)
                    {
                        Jump();
                    }
                }
                else
                {
                    jumpTimer = 0;
                }
            }
            else
            {
                // If we stepped over a cliff or something, set the height at which we started falling
                if (!falling)
                {
                    falling = true;
                    fallStartLevel = myTransform.position.y;
                }

                // If air control is allowed, check movement but don't touch the y component
                if (airControl && playerControl)
                {
                    moveDirection.x = inputX * speed * inputModifyFactor;
                    moveDirection.z = inputY * speed * inputModifyFactor;
                    moveDirection = myTransform.TransformDirection(moveDirection);
                }
            }

            // Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

            // footstep audio?
            float footstepThreshold = .125f;
            if (grounded && (Mathf.Abs(inputX) > footstepThreshold || Mathf.Abs(inputY) > footstepThreshold))
            {
                if (footstepCounter < footstepRate)
                {
                    footstepCounter++;
                }
                else
                {
                    AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.footstepDefaultClips), .25f, .35f, .075f, .1f);
                    footstepCounter = 0;
                }
            }
            else
            {
                footstepCounter = 0;
            }
        }
    }

    void Jump ()
    {
        float jumpSpeedFactor = (hitLavaCounter < hitLavaDur) ? 1.875f : 1f;

        moveDirection.y = (jumpSpeed * jumpSpeedFactor);
        jumpTimer = 0;

        if (hitLavaCounter >= hitLavaDur)
        {
            AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.footstepDefaultClips), 1f, 1.2f, .3f, .325f);
        }
        else
        {
            AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireStartClips), .9f, 1.2f, .3f, .325f);
            GameManager.instance.DealDamageToPlayer(1,Npc.AttackData.DamageType.Special);
            PrefabManager.instance.SpawnPrefab(PrefabManager.instance.lavaBallImpactParticlesPrefab[0],myTransform.position,Quaternion.identity,1f);
        }
        landed = false;

        // explosion when jumping?
        if (SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.ExplosionWhenJumping))
        {
            PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position, Quaternion.identity, 2f);
            PrefabManager.instance.SpawnDamageDeal(myTransform.position, 2f, 1,Npc.AttackData.DamageType.Magic, 10, GameManager.instance.playerFirstPersonDrifter.myTransform, .325f, true, DamageDeal.Target.AI, null, false, false);
            AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.cannonImpactClips), .9f, 1.1f, .3f, .325f);
        }
    }

    void Kick ()
    {
        kickCounter = 0;
        inKick = true;
        createdKickDamageDeal = false;

        // kick audio
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.kickDoClips), 1f, 1.2f, 1f, 1f);
    }

    public void StopKick ()
    {
        inKick = false;
        kickCounter = kickDur;
        createdKickDamageDeal = false;
        HideKickObject();
    }

    void ShowKickObject ()
    {
        if (kickObjects != null && kickObjects.Length > 0)
        {
            if ( kickCounter < (kickDur * kickPrepareFac) )
            {
                kickObjects[0].SetActive(true);
                kickObjects[1].SetActive(false);
            }
            else
            {
                kickObjects[0].SetActive(false);
                kickObjects[1].SetActive(true);
            }
        }
    }

    void HideKickObject()
    {
        if (kickObjects != null && kickObjects.Length > 0)
        {
            for (int i = 0; i < kickObjects.Length; i++)
            {
                kickObjects[i].SetActive(false);
            }
        }
    }

    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit (ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }
 
    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert (float fallDistance)
    {
        //print ("Ouch! Fell " + fallDistance + " units!");   
    }

    //public void PreventInput ()
    //{

    //}
}