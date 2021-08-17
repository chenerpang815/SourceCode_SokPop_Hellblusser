using UnityEngine;

public class FireChargeScript : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    Vector3 sclOriginal, sclTarget, sclCur;
    float sclFacTarget, sclFacCur;
    [HideInInspector] public float scaleFacGrow;
    [HideInInspector] public float scaleFacMultiplier;

    public MeshRenderer myMeshRenderer;
    public Material matA, matB, matEmpty;
    int matRate, matCounter;
    int matIndex;

    public bool isEyeFireCharge;
    public bool isMouthFireCharge;
    public bool regrowWhenInAttackPrepare;
    public bool regrowWhenNpcGotHit;
    public bool onlyCollectibleWhenInAttackPrepare;
    public bool onlyShowWhenInAttackPrepare;
    public bool autoRegrow;
    [HideInInspector] public int autoRegrowDur, autoRegrowCounter;

    public bool canBeCollected;
    [HideInInspector] public bool collected;
    [HideInInspector] public bool empty;
    [HideInInspector] public bool hide;
    [HideInInspector] public NpcCore myNpcCore;
    [HideInInspector] public bool wantsToRegrow;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;

        sclOriginal = myTransform.localScale;
        sclTarget = sclOriginal;
        sclCur = sclTarget;

        scaleFacGrow = 1f;

        matRate = 4;
        matCounter = 0;
        matIndex = 0;

        hide = false;
        collected = false;
        wantsToRegrow = false;
    }

    void Update ()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            // scaling
            float t0 = Time.time * 40f;
            float f0 = 12.5f;
            float s0 = Mathf.Sin(t0) * f0;

            float sclFacExtra = (empty) ? .675f : 1f;

            sclFacTarget = Mathf.Lerp(sclFacTarget, 2f, 20f * Time.deltaTime);
            sclFacCur = sclFacTarget;
            sclTarget = ((sclOriginal * sclFacCur) * scaleFacMultiplier) + (Vector3.one * s0);
            sclCur = Vector3.Lerp(sclCur, sclTarget, 20f * Time.deltaTime);
            myTransform.localScale = ((sclCur * sclFacExtra) * scaleFacGrow);

            // regrow?
            if ( collected && autoRegrow && myNpcCore != null && !wantsToRegrow )
            {
                if ( myNpcCore.curState != NpcCore.State.AttackPrepare && myNpcCore.curState != NpcCore.State.AttackDo  )
                {
                    if (autoRegrowCounter < autoRegrowDur)
                    {
                        autoRegrowCounter++;
                    }
                    else
                    {
                        Regrow();
                        autoRegrowCounter = 0;
                    }
                }
            }
            else
            {
                autoRegrowCounter = 0;
            }

            if ( collected && regrowWhenInAttackPrepare && myNpcCore != null && !wantsToRegrow )
            {
                if ( myNpcCore.curState == NpcCore.State.AttackPrepare )
                {
                    Regrow();
                }
            }

            // hide?
            if (canBeCollected)
            {
                myMeshRenderer.enabled = (!collected && !hide);
                if (collected)
                {
                    empty = true;
                }
            }

            if ( onlyShowWhenInAttackPrepare && myNpcCore != null )
            {
                myMeshRenderer.enabled = (myNpcCore.curState == NpcCore.State.AttackPrepare && myNpcCore.curAttackData.rangedAttackType == Npc.AttackData.RangedAttackType.Laser);
            }

            if ( hide || (myNpcCore != null && (myNpcCore.curState == NpcCore.State.Sleeping || myNpcCore.curState == NpcCore.State.WakeUp)) )
            {
                myMeshRenderer.enabled = false;
            }

            // material
            if (!empty)
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
            else
            {
                myMeshRenderer.material = matEmpty;
            }

            // wants to regrow?
            if ( wantsToRegrow )
            {
                bool canRegrow = true;
                if ( myNpcCore != null && myNpcCore.curState == NpcCore.State.Stunned || myNpcCore.curState == NpcCore.State.Vulnerable )
                {
                    canRegrow = false;
                }
                if ( canRegrow )
                {
                    collected = false;
                    wantsToRegrow = false;
                    if ( isEyeFireCharge && myNpcCore.myInfo.graphics.eyeHasFireCharge )
                    {
                        myNpcCore.lostEyes = false;
                        myNpcCore.PickRandomAttack();
                    }
                }
            }
        }
    }

    public void Regrow ()
    {
        wantsToRegrow = true;
        //collected = false;
    }

    public void Collect ()
    {
        if (!collected)
        {
            // particles
            float whiteOrbScl = .5f;

            /*
            // fire burst blessing?
            if (SetupManager.instance != null && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.FireBurst))
            {
                PrefabManager.instance.SpawnDamageDeal(myTransform.position, 1.5f, 1, Npc.AttackData.DamageType.Magic, 10, HandManager.instance.myTransform, .325f, true, DamageDeal.Target.AI, null,false,false);
                whiteOrbScl = 1.5f;
            }
            */

            PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position, Quaternion.identity, whiteOrbScl);
            PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[1], myTransform.position, Quaternion.identity, 1f);

            // collect!
            GameObject fireCollectO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.fireCollectPrefab, myTransform.position, Quaternion.identity, 1f);
            FireCollectScript fireCollectScript = fireCollectO.GetComponent<FireCollectScript>();
            if (fireCollectScript != null)
            {
                fireCollectScript.targetTransform = GameManager.instance.playerFirstPersonDrifter.myTransform;
                fireCollectScript.npcCollectedBy = null;
            }

            // audio
            AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireStartClips), 1.4f, 1.6f, .1f, .125f);

            // freeze
            SetupManager.instance.SetFreeze(6);

            // store in progress
            switch ( SetupManager.instance.curRunType )
            {
                case SetupManager.RunType.Normal:
                    if (SetupManager.instance.curProgressData.normalRunData.curLevelFiresCleared < SetupManager.instance.curLevelMaxFires)
                    {
                        SetupManager.instance.curProgressData.normalRunData.curLevelFiresCleared++;
                        SetupManager.instance.curProgressData.normalRunData.curRunFiresCleared++;
                    }
                    break;

                case SetupManager.RunType.Endless:
                    if (SetupManager.instance.curProgressData.endlessRunData.curLevelFiresCleared < SetupManager.instance.curLevelMaxFires)
                    {
                        SetupManager.instance.curProgressData.endlessRunData.curLevelFiresCleared++;
                        SetupManager.instance.curProgressData.endlessRunData.curRunFiresCleared++;
                    }
                    break;
            }

            // mouth fire charge?
            if ( isMouthFireCharge && myNpcCore != null )
            {
                myNpcCore.InitGetVulnerable(myNpcCore.myInfo.stats.vulnerableDur);

                // log
                //Debug.Log("UUMMMMM HALLOO HOESSOOOO || " + Time.time.ToString());
            }

            // done
            collected = true;
        }
    }
}
