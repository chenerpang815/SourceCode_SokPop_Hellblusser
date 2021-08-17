using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;
    
    // hands
    [Header("hands")]
    public HandScript[] handScripts;

    // drifter
    [Header("drifter")]
    public FirstPersonDrifter drifterScript;

    // animation
    [Header("animation")]
    public AnimationCurve meleeAttackSwingCurve;
    public AnimationCurve meleeAttackSwingCurveForward;
    public AnimationCurve meleeAttackSwingCurveSide;

    // magic charge audio
    public AudioSource magicChargeAudioSource;

    // layerMask
    [Header("layerMask")]
    public LayerMask fireCollectLayerMask;

    // counters
    [HideInInspector] public int magicFireRate, magicFireCounter;

    private void Awake()
    {
        instance = this;

        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // counters
        magicFireRate = 30;
        magicFireCounter = 0;
    }

    void Start()
    {
        // initialize hands
        for (int i = 0; i < handScripts.Length; i++)
        {
            handScripts[i].handIndex = i;
            handScripts[i].Init();
        }

        // create equipment from saveFile
        int bodyEquipmentIndex = SetupManager.instance.runDataRead.curBodyEquipmentIndex;
        int braceletEquipmentIndex = SetupManager.instance.runDataRead.curBraceletEquipmentIndex;
        int ringEquipmentIndex = SetupManager.instance.runDataRead.curRingEquipmentIndex;
        int weaponEquipmentIndex = SetupManager.instance.runDataRead.curWeaponEquipmentIndex;
        if (bodyEquipmentIndex != -1)
        {
            EquipItem((EquipmentDatabase.Equipment)bodyEquipmentIndex);
        }
        if (braceletEquipmentIndex != -1)
        {
            EquipItem((EquipmentDatabase.Equipment)braceletEquipmentIndex);
        }
        if (ringEquipmentIndex != -1)
        {
            EquipItem((EquipmentDatabase.Equipment)ringEquipmentIndex);
        }
        if (weaponEquipmentIndex != -1)
        {
            EquipItem((EquipmentDatabase.Equipment)weaponEquipmentIndex);
        }

        // drained out achievement?
        if (SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Drainer))
        {
            if ((bodyEquipmentIndex != -1 && ((EquipmentDatabase.Equipment)bodyEquipmentIndex == EquipmentDatabase.Equipment.CloakOfSadness)) && (weaponEquipmentIndex != -1 && ((EquipmentDatabase.Equipment)weaponEquipmentIndex == EquipmentDatabase.Equipment.DrainBlade)) )
            {
                AchievementHelper.UnlockAchievement("ACHIEVEMENT_DRAINEDOUT");
            }
        }
    }

    void HandDefaultVisuals ()
    {
        for (int i = 0; i < handScripts.Length; i++)
        {
            handScripts[i].SetHandState((i == 0) ? HandScript.HandState.Default : HandScript.HandState.Hold);
        }
    }

    void Update ()
    {
        if ( SetupManager.instance != null && SetupManager.instance.inFreeze )
        {
            AudioManager.instance.magicCollectAudioSource.volume = 0f;
            if (GameManager.instance != null && GameManager.instance.inGameWait)
            {
                HandDefaultVisuals();
            }
            return;
        }

        if (GameManager.instance != null && GameManager.instance.inGameWait)
        {
            AudioManager.instance.magicCollectAudioSource.volume = 0f;
            HandDefaultVisuals();
            return;
        }

        bool inCollectMode = false;
        bool collectingFire = false;

        //if ( !GameManager.instance.inFreeze )
        {
            /*
            if ( Input.GetKeyDown(KeyCode.Q) )
            {
                handScripts[0].SetMagicMode((handScripts[0].curMagicMode == HandScript.MagicMode.Out) ? HandScript.MagicMode.In : HandScript.MagicMode.Out);
                GameManager.instance.SetPlayerFireButtonFlicker(2);
            }
            */

            for (int i = 0; i < handScripts.Length; i++)
            {
                bool holdingObject = (handScripts[i].objectHoldingScript != null);
                bool mousePressInputCheckFor = (i == 0) ? InputManager.instance.magicAttackPressed : InputManager.instance.meleeAttackPressed;
                bool mouseHoldInputCheckFor = (i == 0) ? InputManager.instance.magicAttackHold : InputManager.instance.meleeAttackHold;
                bool mouseReleaseInputCheckFor = (i == 0) ? InputManager.instance.magicAttackReleased : InputManager.instance.meleeAttackReleased;

                //float attackThreshold = .125f;
                //bool aboveAttackThreshold = (i == 0 || InputManager.instance.moveDirection.y > -attackThreshold);

                bool keyboardBlocking = InputManager.instance.keyboardBlock;
                bool gamepadBlocking = InputManager.instance.gamepadBlock;

                bool alreadyBlocking = (keyboardBlocking || gamepadBlocking);

                bool inputGiven = (mousePressInputCheckFor && !alreadyBlocking && !SetupManager.instance.inFreeze);
                bool inputHold = (mouseHoldInputCheckFor && !alreadyBlocking && !SetupManager.instance.inFreeze);
                bool inputReleased = (mouseReleaseInputCheckFor && !alreadyBlocking && !SetupManager.instance.inFreeze);

                bool playerCanAttack = (GameManager.instance.playerFirstPersonDrifter.grounded || (!GameManager.instance.playerFirstPersonDrifter.grounded && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Acrobat)));
                if ( GameManager.instance.playerFirstPersonDrifter.playerBlocking )
                {
                    playerCanAttack = false;
                }
                if ( GameManager.instance.playerHurt || SetupManager.instance.runDataRead.playerDead )
                {
                    playerCanAttack = false;
                }
                if ( GameManager.instance.playerFirstPersonDrifter.inKick || GameManager.instance.playerFirstPersonDrifter.playerStunned)
                {
                    playerCanAttack = false;
                    inputGiven = false;
                    inputHold = false;
                    inputReleased = false;
                }

                if (!holdingObject)
                {
                    // HOLD INPUT
                    if (playerCanAttack && handScripts[i].justFiredCounter >= handScripts[i].justFiredDur)
                    {
                        if (i == 0 && inputHold)
                        {
                            handScripts[i].Charge();
                            switch (handScripts[i].magicChargeIndex)
                            {
                                case 0:
                                    if ( handScripts[i].curHandState != HandScript.HandState.MagicCharge )
                                    {
                                        StartMagicChargeAudio();
                                    }
                                    handScripts[i].SetHandState(HandScript.HandState.MagicCharge);
                                    break;

                                case 1:
                                    handScripts[i].SetHandState(HandScript.HandState.MagicReady);
                                    break;

                                case 2:
                                    handScripts[i].SetHandState(HandScript.HandState.MagicCollect);

                                    inCollectMode = true;
                                    if (inputHold)
                                    {
                                        collectingFire = true;
                                    }
                                    break;
                            }
                            /*
                            if ( handScripts[i].magicChargeIndex == 2 && GameManager.instance.playerFireCount >= SetupManager.instance.curProgressData.playerFireMax )
                            {
                                inCollectMode = false;
                                collectingFire = false;
                                handScripts[i].SetHandState(HandScript.HandState.MagicCharge);
                                handScripts[i].magicChargeIndex = 0;
                                handScripts[i].magicChargeCounter = 0;
                            }
                            */
                        }

                        // RELEASE INPUT
                        if (i == 0 && inputReleased && !inputHold )
                        {
                            switch (handScripts[i].magicChargeIndex)
                            {
                                case 0:
                                    if (handScripts[i].magicChargeCounter > 0)
                                    {
                                        handScripts[i].CastMagic(0,true,false);
                                    }
                                    break;

                                case 1:
                                    handScripts[i].CastMagic(0,true,false);
                                    break;

                                case 2:
                                    break;
                            }

                            StopMagicChargeAudio();
                            handScripts[i].StopCharge();
                        }
                    }
                }
                if (holdingObject)
                {
                    handScripts[i].SetHandState(HandScript.HandState.Hold);

                    bool canInitAttack = (playerCanAttack && !handScripts[i].inMeleeAttack);
                    if (handScripts[i].inMeleeAttack && handScripts[i].meleeAttackCounter > (handScripts[i].meleeAttackDur * .825f))
                    {
                        canInitAttack = true;
                    }
                    if (canInitAttack && inputGiven)
                    {
                        handScripts[i].InitMeleeAttack();

                        // melee attack audio
                        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.meleeAttackClips), 1.4f, 1.8f, .325f, .35f);
                        //AudioManager.instance.PlaySound(meleeAudioSource, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.meleeAttackClips), 1.4f, 1.8f, .325f, .35f);
                    }
                }
                if (!inputGiven && !inputHold && !holdingObject)
                {
                    handScripts[i].SetHandState(HandScript.HandState.Default);
                }

                if (handScripts[i].justFiredCounter < handScripts[i].justFiredDur)
                {
                    handScripts[i].SetHandState(HandScript.HandState.MagicRelease);
                }

                if (i == 1 && GameManager.instance.playerFirstPersonDrifter.playerBlocking)
                {
                    handScripts[i].SetHandState(HandScript.HandState.Block);
                }

                if ( GameManager.instance.playerFirstPersonDrifter.playerStunned )
                {
                    handScripts[i].SetHandState(HandScript.HandState.Stunned);
                }

                if ( GameManager.instance.playerHurt )
                {
                    handScripts[i].SetHandState(HandScript.HandState.Hurt);
                }
            }

            // collecting fire?
            if ( inCollectMode )
            {
                float cDst = 14f;
                Vector3 p0 = GameManager.instance.mainCameraTransform.position;
                Vector3 p1 = p0 + (GameManager.instance.mainCameraTransform.forward * cDst);
                RaycastHit pHit;
                if ( Physics.Linecast(p0,p1,out pHit,fireCollectLayerMask) )
                {
                    Transform trHit = pHit.transform;

                    // hit fireScript?
                    if (trHit.GetComponent<FireScript>() != null)
                    {
                        FireScript hitFireScript = trHit.GetComponent<FireScript>();
                        if (hitFireScript != null)
                        {
                            // highlight fire
                            hitFireScript.Highlight();

                            // collect fire
                            if (collectingFire )
                            {
                                if (SetupManager.instance.runDataRead.playerFireCur < SetupManager.instance.runDataRead.playerFireMax)
                                {
                                    //hitFireScript.Collect();

                                    if (hitFireScript.myFireType == FireScript.FireType.Normal)
                                    {
                                        if (hitFireScript.autoClear)
                                        {
                                            hitFireScript.Douse();
                                        }
                                        else
                                        {
                                            hitFireScript.Collect(true,null);
                                        }
                                    }
                                }
                                else // can't collect because we're full?
                                {
                                    if ( !SetupManager.instance.curProgressData.persistentData.sawMaxFirePopup )
                                    {
                                        SetupManager.instance.SetTutorialPopup(SetupManager.instance.UIPopupBaseCol + "you can only hold 3 fires!" + "\n" + "shoot some to make room!",60);
                                        SetupManager.instance.curProgressData.persistentData.sawMaxFirePopup = true;
                                    }

                                    GameManager.instance.SetPlayerFireFlicker(3);
                                }
                            }
                        }
                    }

                    // hit fireChargeScript?
                    if (trHit.GetComponent<FireChargeScript>() != null)
                    {
                        FireChargeScript hitFireChargeScript = trHit.GetComponent<FireChargeScript>();
                        if (hitFireChargeScript != null && collectingFire && !hitFireChargeScript.hide )
                        {
                            bool canCollectFireCharge = true;
                            if ( hitFireChargeScript.onlyCollectibleWhenInAttackPrepare )
                            {
                                if ( hitFireChargeScript.myNpcCore != null && hitFireChargeScript.myNpcCore.curState != NpcCore.State.AttackPrepare )
                                {
                                    canCollectFireCharge = false;
                                }
                            }
                            if (canCollectFireCharge)
                            {
                                hitFireChargeScript.Collect();
                            }
                        }
                    }
                }
            }

            // magic collect audio
            if ( AudioManager.instance.magicCollectAudioSource != null )
            {
                float pitchTarget = (SetupManager.instance.runDataRead.playerFireCur < SetupManager.instance.runDataRead.playerFireMax) ? .6f : .3f;
                float volTarget = (inCollectMode) ? .3f : 0f;
                float volLerpie = (20f * Time.deltaTime);
                AudioManager.instance.magicCollectAudioSource.volume = Mathf.Lerp(AudioManager.instance.magicCollectAudioSource.volume,volTarget * SetupManager.instance.sfxVolFactor,volLerpie);
                AudioManager.instance.magicCollectAudioSource.pitch = Mathf.Lerp(AudioManager.instance.magicCollectAudioSource.pitch, pitchTarget, volLerpie);
            }
        }
    }

    public void EquipItem ( EquipmentDatabase.Equipment _equipment )
    {
        int braceletArmIndex = 0;
        int ringArmIndex = 0;
        int weaponArmIndex = 1;
        EquipmentDatabase.EquipmentData equipmentData = EquipmentDatabase.instance.GetEquipmentData(_equipment);
        switch ( equipmentData.slot )
        {
            case EquipmentDatabase.Slot.Body:
                for (int i = 0; i < handScripts.Length; i++)
                {
                    handScripts[i].UpdateSleeveMaterial();
                }
                break;

            case EquipmentDatabase.Slot.Bracelet:

                float braceletScl = .04f;

                if (handScripts[braceletArmIndex].braceletEquippedObject != null)
                {
                    Destroy(handScripts[braceletArmIndex].braceletEquippedObject);
                }

                GameObject braceletO = PrefabManager.instance.SpawnPrefabAsGameObject(equipmentData.visualsObject, Vector3.zero, Quaternion.identity, braceletScl);
                Transform braceletTr = braceletO.transform;
                braceletTr.parent = handScripts[braceletArmIndex].myTransform;

                handScripts[braceletArmIndex].braceletEquippedTransform = braceletTr;
                handScripts[braceletArmIndex].braceletEquippedObject = braceletO;

                break;

            case EquipmentDatabase.Slot.Ring:

                float ringScl = .0325f;

                if ( handScripts[ringArmIndex].ringEquippedObject != null )
                {
                    Destroy(handScripts[ringArmIndex].ringEquippedObject);
                }

                GameObject ringO = PrefabManager.instance.SpawnPrefabAsGameObject(equipmentData.visualsObject, Vector3.zero, Quaternion.identity, ringScl);
                Transform ringTr = ringO.transform;
                ringTr.parent = handScripts[ringArmIndex].myTransform;

                handScripts[ringArmIndex].ringEquippedTransform = ringTr;
                handScripts[ringArmIndex].ringEquippedObject = ringO;

                break;

            case EquipmentDatabase.Slot.Weapon:

                handScripts[weaponArmIndex].ReleaseObject(true);
                GameObject swordO = PrefabManager.instance.SpawnPrefabAsGameObject(equipmentData.visualsObject, Vector3.zero, Quaternion.identity, 1f);
                ObjectScript swordObjectScript = swordO.GetComponent<ObjectScript>();
                handScripts[weaponArmIndex].GrabObject(swordObjectScript);

                break;
        }
    }

    public void StartMagicChargeAudio ()
    {
        if ( magicChargeAudioSource != null )
        {
            magicChargeAudioSource.Stop();
            magicChargeAudioSource.pitch = .8f;
            magicChargeAudioSource.volume = (.125f * SetupManager.instance.sfxVolFactor);
            magicChargeAudioSource.clip = AudioManager.instance.magicStartChargeClips[0];
            magicChargeAudioSource.Play();
        }
    }

    public void StopMagicChargeAudio ()
    {
        if ( magicChargeAudioSource != null )
        {
            magicChargeAudioSource.Stop();
        }
    }
}
