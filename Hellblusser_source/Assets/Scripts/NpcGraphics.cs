using UnityEngine;
using System.Collections.Generic;

public class NpcGraphics : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // content
    [Header("graphics")]
    public Transform graphicsTransform;
    public GameObject graphicsObject;
    Vector3 graphicsSclOriginal;

    // rotation
    Vector3 graphicsLocalRotOriginal, graphicsLocalRotTarget, graphicsLocalRotCur; 

    // core
    [Header("core")]
    public NpcCore myCore;

    // eyes
    [Header("eye")]
    public Transform[] eyeTransforms;

    // mouth fire charge objects
    [HideInInspector] public List<GameObject> mouthFireChargeObjects;
    [HideInInspector] public List<Transform> mouthFireChargeTransforms;
    [HideInInspector] public List<FireChargeScript> mouthFireChargeScripts;

    // eye fire charge objects
    [HideInInspector] public List<GameObject> eyeFireChargeObjects;
    [HideInInspector] public List<Transform> eyeFireChargeTransforms;
    [HideInInspector] public List<FireChargeScript> eyeFireChargeScripts;

    // body
    [Header("body")]
    public Transform bodyTransform;
    public MeshRenderer[] bodyMeshRenderers;
    [HideInInspector] public Material[] bodyMatsOriginal;
    [HideInInspector] public Vector3 bodySclOriginal, bodySclTarget, bodySclCur;

    // robe
    [Header("robe")]
    public MeshRenderer[] robeMeshRenderers;

    // head
    [Header("head")]
    public Transform headTransform;
    public MeshRenderer[] headMeshRenderers;
    [HideInInspector] public Material[] headMatsOriginal;
    
    // legs
    [HideInInspector] public List<Vector3[]> legPoints;
    [HideInInspector] public LineRenderer[] legLines;
    int stepRate, stepCounter;
    int legTurn;
    [HideInInspector] public Material legMatOriginal;

    // arms
    [HideInInspector] public List<Vector3[]> armPoints;
    [HideInInspector] public LineRenderer[] armLines;
    [HideInInspector] public Material armMatOriginal;

    // mouth
    public Transform[] mouthTransforms;
    Vector3[] mouthRotOriginal, mouthRotTarget, mouthRotCur;
    public Transform mouthProjectileSpawnTransform;

    // feet
    Transform[] feetTransforms;
    [HideInInspector] public GameObject[] feetObjects;
    [HideInInspector] public MeshRenderer[] feetMeshRenderers;
    [HideInInspector] public Material feetMatOriginal;

    // hands
    Transform[] handTransforms;
    [HideInInspector] public GameObject[] handObjects;
    [HideInInspector] public MeshRenderer[] handMeshRenderers;
    [HideInInspector] public Material handMatOriginal;

    // wings
    Transform[] wingTransforms;
    [HideInInspector] public GameObject[] wingObjects;
    [HideInInspector] public MeshRenderer[] wingMeshRenderers;
    [HideInInspector] public Material wingMatOriginal;

    // tail
    [HideInInspector] public Vector3[] tailPoints;
    [HideInInspector] public LineRenderer tailLine;
    [HideInInspector] public Material tailMatOriginal;

    // equipment
    [HideInInspector] public GameObject[] equipmentObjects;
    [HideInInspector] public Transform[] equipmentTransforms;

    // directions
    Vector3 graphicsUp, graphicsForward, graphicsRight;
    Vector3 bodyUp, bodyForward, bodyRight;

    // alerted
    [HideInInspector] public float alertedFac;

    // attack prepare
    [HideInInspector] public float attackPrepareFac, attackPrepareFacReal;
    [HideInInspector] public Quaternion attackPrepareRot;

    // attack do
    [HideInInspector] public float attackDoFac;

    // time
    float timeOffset;

    // flying
    [HideInInspector] public float flyHeightUse;
    [HideInInspector] public float flyOff;

    // init
    [HideInInspector] public bool initialized;

    public void Init ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // create graphics
        CreateGraphics();

        // graphics scale
        graphicsSclOriginal = graphicsTransform.localScale;

        // graphics local rotation
        graphicsLocalRotOriginal = graphicsTransform.localRotation.eulerAngles;
        graphicsLocalRotTarget = graphicsLocalRotOriginal;
        graphicsLocalRotCur = graphicsLocalRotTarget;

        // animation
        stepRate = myCore.myInfo.graphics.stepRate;
        stepCounter = 0;
        legTurn = 0;

        // time
        timeOffset = TommieRandom.instance.RandomRange(-10f,10f);

        // flying
        flyHeightUse = myCore.myInfo.graphics.flyHeight;

        // done
        initialized = true;
    }

    public void SetGraphicsObject ( bool _to )
    {
        if ( graphicsObject != null )
        {
            graphicsObject.SetActive(_to);
        }
        if ( wingMeshRenderers != null && wingMeshRenderers.Length > 0 )
        {
            for ( int i = 0; i < wingMeshRenderers.Length; i ++ )
            {
                wingMeshRenderers[i].enabled = _to;
            }
        }
        if (feetMeshRenderers != null && feetMeshRenderers.Length > 0)
        {
            for (int i = 0; i < feetMeshRenderers.Length; i++)
            {
                feetMeshRenderers[i].enabled = _to;
            }
        }
        if (handMeshRenderers != null && handMeshRenderers.Length > 0)
        {
            for (int i = 0; i < handMeshRenderers.Length; i++)
            {
                handMeshRenderers[i].enabled = _to;
            }
        }
        if (equipmentObjects != null && equipmentObjects.Length > 0)
        {
            for (int i = 0; i < equipmentObjects.Length; i++)
            {
                equipmentObjects[i].SetActive(_to);
            }
        }
    }

    void CreateGraphics ()
    {
        // store eye transforms
        if (myCore != null && eyeTransforms != null && eyeTransforms.Length > 0)
        {
            myCore.eyeTransforms = new Transform[2];
            for (int i = 0; i < eyeTransforms.Length; i++)
            {
                myCore.eyeTransforms[i] = eyeTransforms[i];
            }
        }

        // eye fire charge object(s)
        if (myCore.myInfo.graphics.eyeHasFireCharge)
        {
            eyeFireChargeObjects = new List<GameObject>();
            eyeFireChargeTransforms = new List<Transform>();
            eyeFireChargeScripts = new List<FireChargeScript>();
            for (int i = 0; i < eyeTransforms.Length; i++)
            {
                GameObject eyeFireChargeO = PrefabManager.instance.SpawnPrefabAsGameObject(myCore.myInfo.graphics.eyeFireChargePrefab, eyeTransforms[i].position, Quaternion.identity, 1f);
                Transform eyeFireChargeTr = eyeFireChargeO.transform;
                //eyeFireChargeTr.parent = myTransform;
                FireChargeScript eyeFireChargeScript = eyeFireChargeO.GetComponent<FireChargeScript>();
                eyeFireChargeScript.scaleFacMultiplier = myCore.myInfo.graphics.eyeFireChargeScaleFactor;
                eyeFireChargeScript.myNpcCore = myCore;
                eyeFireChargeScript.regrowWhenNpcGotHit = true;
                eyeFireChargeScript.autoRegrow = true;
                eyeFireChargeScript.autoRegrowDur = 420;
                eyeFireChargeScript.isEyeFireCharge = true;
                float scl = .05f;
                eyeFireChargeScript.GetComponent<BoxCollider>().size = new Vector3(scl,scl,scl);
                eyeFireChargeObjects.Add(eyeFireChargeO);
                eyeFireChargeTransforms.Add(eyeFireChargeTr);
                eyeFireChargeScripts.Add(eyeFireChargeScript);
            }
        }

        // mouth fire charge object(s)
        if ( myCore.myInfo.graphics.mouthHasFireCharge )
        {
            mouthFireChargeObjects = new List<GameObject>();
            mouthFireChargeTransforms = new List<Transform>();
            mouthFireChargeScripts = new List<FireChargeScript>();
            GameObject mouthFireChargeO = PrefabManager.instance.SpawnPrefabAsGameObject(myCore.myInfo.graphics.eyeFireChargePrefab, mouthProjectileSpawnTransform.position, Quaternion.identity, 1f);
            Transform mouthFireChargeTr = mouthFireChargeO.transform;
            //mouthFireChargeTr.parent = myTransform;
            FireChargeScript mouthFireChargeScript = mouthFireChargeO.GetComponent<FireChargeScript>();
            mouthFireChargeScript.scaleFacMultiplier = myCore.myInfo.graphics.mouthFireChargeScaleFactor;
            mouthFireChargeScript.onlyCollectibleWhenInAttackPrepare = true;
            mouthFireChargeScript.onlyShowWhenInAttackPrepare = true;
            mouthFireChargeScript.myNpcCore = myCore;
            mouthFireChargeScript.regrowWhenInAttackPrepare = true;
            mouthFireChargeScript.isMouthFireCharge = true;
            float scl = .05f;
            mouthFireChargeScript.GetComponent<BoxCollider>().size = new Vector3(scl, scl, scl);
            mouthFireChargeObjects.Add(mouthFireChargeO);
            mouthFireChargeTransforms.Add(mouthFireChargeTr);
            mouthFireChargeScripts.Add(mouthFireChargeScript);
        }

        // body
        bodyMatsOriginal = new Material[bodyMeshRenderers.Length];
        for ( int i =0; i < bodyMeshRenderers.Length; i ++ )
        {
            bodyMatsOriginal[i] = bodyMeshRenderers[i].material;
        }
        if ( myCore.myInfo.graphics.hasBody && bodyTransform != null )
        {
            bodySclOriginal = bodyTransform.localScale;
            bodySclTarget = bodySclOriginal;
            bodySclCur = bodySclTarget;
        }

        // head
        headMatsOriginal = new Material[headMeshRenderers.Length];
        for (int i = 0; i < headMeshRenderers.Length; i++)
        {
            headMatsOriginal[i] = headMeshRenderers[i].material;
        }

        // legs
        if (myCore.myInfo.graphics.hasLegs)
        {
            legPoints = new List<Vector3[]>();
            legLines = new LineRenderer[myCore.myInfo.graphics.legCount];
            for (int i = 0; i < myCore.myInfo.graphics.legCount; i++)
            {
                GameObject legO = new GameObject("legO_" + i.ToString());
                Transform legTr = legO.transform;
                legTr.parent = graphicsTransform;
                BasicFunctions.ResetTransform(legTr);

                LineRenderer legLr = legO.AddComponent<LineRenderer>();
                legLr.useWorldSpace = true;
                legLr.positionCount = 3;
                legLr.numCapVertices = 2;

                float legW = myCore.myInfo.graphics.legWidth;
                legLr.startWidth = legW;
                legLr.endWidth = legW;

                legLr.material = myCore.myInfo.graphics.legMat;
                legLr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

                legPoints.Add(new Vector3[3]);
                legLines[i] = legLr;
            }
            legMatOriginal = myCore.myInfo.graphics.legMat;
        }

        // arms
        if (myCore.myInfo.graphics.hasArms)
        {
            armPoints = new List<Vector3[]>();
            armLines = new LineRenderer[myCore.myInfo.graphics.armCount];
            for (int i = 0; i < myCore.myInfo.graphics.armCount; i++)
            {
                GameObject armO = new GameObject("armO_" + i.ToString());
                Transform armTr = armO.transform;
                armTr.parent = graphicsTransform;
                BasicFunctions.ResetTransform(armTr);

                LineRenderer armLr = armO.AddComponent<LineRenderer>();
                armLr.useWorldSpace = true;
                armLr.positionCount = 3;
                armLr.numCapVertices = 2;

                float armW = myCore.myInfo.graphics.armWidth;
                armLr.startWidth = armW;
                armLr.endWidth = armW;

                armLr.material = myCore.myInfo.graphics.armMat;
                armLr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

                armPoints.Add(new Vector3[3]);
                armLines[i] = armLr;
            }
            armMatOriginal = myCore.myInfo.graphics.armMat;
        }

        // tail
        if ( myCore.myInfo.graphics.hasTail )
        {
            GameObject tailO = new GameObject("tailO");
            Transform tailTr = tailO.transform;
            tailTr.parent = graphicsTransform;
            BasicFunctions.ResetTransform(tailTr);

            LineRenderer tailLr = tailO.AddComponent<LineRenderer>();
            tailLr.useWorldSpace = true;
            tailLr.positionCount = 3;
            tailLr.numCapVertices = 2;

            float tailW = myCore.myInfo.graphics.tailWidth;
            tailLr.startWidth = tailW;
            tailLr.endWidth = tailW * myCore.myInfo.graphics.tailWidthTipFac;

            tailLr.material = myCore.myInfo.graphics.tailMat;

            tailPoints = new Vector3[3];
            tailLine = tailLr;

            tailMatOriginal = tailLine.material;
        }

        // feet
        if ( myCore.myInfo.graphics.hasFeet )
        {
            int feetCount = myCore.myInfo.graphics.legCount;
            feetTransforms = new Transform[feetCount];
            feetObjects = new GameObject[feetCount];
            feetMeshRenderers = new MeshRenderer[feetCount];
            for (int i = 0; i < feetCount; i++)
            {
                GameObject feetO = new GameObject("feetO");
                Transform feetTr = feetO.transform;
                feetTr.parent = null;
                BasicFunctions.ResetTransform(feetTr);

                feetTr.localScale = Vector3.one * myCore.myInfo.graphics.feetScale;

                MeshRenderer feetMr = feetO.AddComponent<MeshRenderer>();
                feetMr.material = myCore.myInfo.graphics.feetMat;

                MeshFilter feetMf = feetO.AddComponent<MeshFilter>();
                feetMf.mesh = myCore.myInfo.graphics.feetMesh;

                feetTransforms[i] = feetTr;
                feetObjects[i] = feetO;
                feetMeshRenderers[i] = feetMr;
            }
            feetMatOriginal = myCore.myInfo.graphics.feetMat;
        }

        // hands
        if (myCore.myInfo.graphics.hasHand)
        {
            int handCount = myCore.myInfo.graphics.armCount;
            handTransforms = new Transform[handCount];
            handObjects = new GameObject[handCount];
            handMeshRenderers = new MeshRenderer[handCount];
            for (int i = 0; i < handCount; i++)
            {
                GameObject handO = new GameObject("handO");
                Transform handTr = handO.transform;
                handTr.parent = null;
                BasicFunctions.ResetTransform(handTr);

                handTr.localScale = Vector3.one * myCore.myInfo.graphics.handScale;

                if ( i == 0 )
                {
                    Vector3 handScl3 = handTr.localScale;
                    handScl3.x *= -1f;
                    handTr.localScale = handScl3;
                }

                MeshRenderer handMr = handO.AddComponent<MeshRenderer>();
                handMr.material = myCore.myInfo.graphics.handMat;

                MeshFilter handMf = handO.AddComponent<MeshFilter>();
                handMf.mesh = myCore.myInfo.graphics.handMesh;

                handTransforms[i] = handTr;
                handObjects[i] = handO;
                handMeshRenderers[i] = handMr;
            }
            handMatOriginal = myCore.myInfo.graphics.handMat;
        }

        // wings
        if (myCore.myInfo.graphics.hasWings)
        {
            int wingCount = 2;
            wingTransforms = new Transform[wingCount];
            wingObjects = new GameObject[wingCount];
            wingMeshRenderers = new MeshRenderer[wingCount];
            for (int i = 0; i < wingCount; i++)
            {
                GameObject wingO = new GameObject("wingO");
                Transform wingTr = wingO.transform;
                wingTr.parent = null;
                BasicFunctions.ResetTransform(wingTr);

                wingTr.localScale = Vector3.one * myCore.myInfo.graphics.wingScl;

                MeshRenderer wingMr = wingO.AddComponent<MeshRenderer>();
                wingMr.material = myCore.myInfo.graphics.wingMat;

                MeshFilter wingMf = wingO.AddComponent<MeshFilter>();
                wingMf.mesh = myCore.myInfo.graphics.wingMesh;

                wingTransforms[i] = wingTr;
                wingObjects[i] = wingO;
                wingMeshRenderers[i] = wingMr;
            }
            wingMatOriginal = myCore.myInfo.graphics.wingMat;
        }

        // mouth
        if ( myCore.myInfo.graphics.hasMouth )
        {
            mouthRotOriginal = new Vector3[mouthTransforms.Length];
            mouthRotTarget = new Vector3[mouthTransforms.Length];
            mouthRotCur = new Vector3[mouthTransforms.Length];
            for ( int i = 0; i < mouthTransforms.Length; i ++ )
            {
                Vector3 r = mouthTransforms[i].localRotation.eulerAngles;
                mouthRotOriginal[i] = r;
                mouthRotTarget[i] = r;
                mouthRotCur[i] = r;
            }
        }

        // equipment
        if ( myCore.myInfo.graphics.hasEquipment )
        {
            equipmentObjects = new GameObject[myCore.myInfo.graphics.equipments.Length];
            equipmentTransforms = new Transform[myCore.myInfo.graphics.equipments.Length];
            for ( int i = 0; i < myCore.myInfo.graphics.equipments.Length; i ++ )
            {
                EquipmentDatabase.EquipmentData curEquipmentData = EquipmentDatabase.instance.GetEquipmentData(myCore.myInfo.graphics.equipments[i]);
                float sclUse = 1f;
                switch ( curEquipmentData.slot )
                {
                    case EquipmentDatabase.Slot.Body: sclUse = EquipmentDatabase.instance.bodyShopSclFactor; break;
                    case EquipmentDatabase.Slot.Bracelet: sclUse = EquipmentDatabase.instance.braceletShopSclFactor; break;
                    case EquipmentDatabase.Slot.Ring: sclUse = EquipmentDatabase.instance.ringShopSclFactor; break;
                    case EquipmentDatabase.Slot.Weapon: sclUse = EquipmentDatabase.instance.weaponShopSclFactor; break;
                }
                GameObject equipmentO = PrefabManager.instance.SpawnPrefabAsGameObject(curEquipmentData.visualsObject,myTransform.position,Quaternion.identity, sclUse * .675f);
                Transform equipmentTr = equipmentO.transform;

                equipmentObjects[i] = equipmentO;
                equipmentTransforms[i] = equipmentTr;
            }
        }
    }

    void Update ()
    {
        if (initialized)
        {
            if (!SetupManager.instance.inFreeze)
            {
                // dynamic fly height?
                if ( myCore.myInfo.graphics.flying )
                {
                    float flyHeightAdd = (.75f * Time.deltaTime);
                    float flyHeightTarget = myCore.myInfo.graphics.flyHeight;
                    if ( myCore.curState == NpcCore.State.EnterNextStage )
                    {
                        flyHeightTarget = myCore.myInfo.graphics.flyHeightEnterNextStage;
                    }

                    if ( flyHeightUse < flyHeightTarget )
                    {
                        flyHeightUse += flyHeightAdd;
                    }
                    if ( flyHeightUse > flyHeightTarget )
                    {
                        flyHeightUse -= flyHeightAdd;
                    }
                    if ( BasicFunctions.IsApproximately(flyHeightUse,flyHeightTarget,.0125f) )
                    {
                        flyHeightUse = flyHeightTarget;
                    }
                }

                // directions
                graphicsUp = graphicsTransform.up.normalized;
                graphicsForward = graphicsTransform.forward.normalized;
                graphicsRight = graphicsTransform.right.normalized;

                bodyUp = bodyTransform.up.normalized;
                bodyForward = bodyTransform.forward.normalized;
                bodyRight = bodyTransform.right.normalized;

                // legs?
                if (myCore.myInfo.graphics.hasLegs)
                {
                    UpdateLegs();
                }

                // graphics
                if (graphicsTransform != null)
                {
                    UpdateGraphics();
                }

                // arms
                if (myCore.myInfo.graphics.hasArms)
                {
                    UpdateArms();
                }

                // body
                if (myCore.myInfo.graphics.hasBody)
                {
                    UpdateBody();
                }

                // head
                if (myCore.myInfo.graphics.hasHead)
                {
                    UpdateHead();
                }

                // tail
                if (myCore.myInfo.graphics.hasTail)
                {
                    UpdateTail();
                }

                // feet
                if (myCore.myInfo.graphics.hasFeet)
                {
                    UpdateFeet();
                }

                // hand
                if (myCore.myInfo.graphics.hasHand)
                {
                    UpdateHand();
                }

                //// wings
                //if (myCore.myInfo.graphics.hasWings)
                //{
                //    UpdateWings();
                //}

                // mouth
                if (myCore.myInfo.graphics.hasMouth)
                {
                    UpdateMouth();
                }

                // animation
                if (myCore.myUnit.moving)
                {
                    if (stepCounter < stepRate)
                    {
                        stepCounter++;
                    }
                    else
                    {
                        legTurn = (legTurn == 0) ? 1 : 0;
                        stepCounter = 0;
                    }
                }
                else
                {
                    stepCounter = 0;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            // fire charge objects
            if (myCore.myInfo.graphics.eyeHasFireCharge || myCore.myInfo.graphics.mouthHasFireCharge)
            {
                UpdateFireCharges();
            }

            // wings
            if (myCore.myInfo.graphics.hasWings)
            {
                UpdateWings();
            }
        }
    }

    public void ResetFireCharges ()
    {
        // eye fire charge(s)
        if (myCore.myInfo.graphics.eyeHasFireCharge)
        {
            for (int i = 0; i < eyeFireChargeScripts.Count; i++)
            {
                eyeFireChargeScripts[i].collected = false;
            }
        }

        // mouth fire charge(s)
        if (myCore.myInfo.graphics.mouthHasFireCharge)
        {
            for (int i = 0; i < mouthFireChargeScripts.Count; i++)
            {
                mouthFireChargeScripts[i].collected = false;
            }
        }
    }

    void UpdateFireCharges ()
    {
        // eye fire charge(s)
        if ( myCore.myInfo.graphics.eyeHasFireCharge )
        {
            bool eyesGone = true;
            for ( int i = 0; i < eyeFireChargeObjects.Count; i ++ )
            {
                Vector3 p = eyeTransforms[i].position;
                eyeFireChargeTransforms[i].position = p;

                Quaternion r = Quaternion.identity;
                eyeFireChargeTransforms[i].rotation = r;

                eyeFireChargeScripts[i].empty = false;

                if ( !eyeFireChargeScripts[i].collected )
                {
                    eyesGone = false;
                }
            }
            if ( eyesGone )
            {
                myCore.LoseEyes();
            }
        }

        // mouth fire charge(s)
        if (myCore.myInfo.graphics.mouthHasFireCharge )
        {
            float mouthFireGrowFac = 1f;
            if ( myCore.curState == NpcCore.State.AttackPrepare )
            {
                mouthFireGrowFac = BasicFunctions.ConvertRange(myCore.attackPrepareCounter,0f,myCore.curAttackData.attackPrepareDur,0f,1f);
            }

            for (int i = 0; i < mouthFireChargeTransforms.Count; i++)
            {
                Vector3 p = mouthProjectileSpawnTransform.position;
                mouthFireChargeTransforms[i].position = p;

                Quaternion r = Quaternion.identity;
                mouthFireChargeTransforms[i].rotation = r;

                mouthFireChargeScripts[i].empty = false;

                mouthFireChargeScripts[i].scaleFacGrow = (1f + (mouthFireGrowFac * myCore.myInfo.graphics.mouthFireChargeGrowTarget));

                bool eyesAreCollected = true;
                for ( int ii = 0; ii < eyeFireChargeScripts.Count; ii ++ )
                {
                    if ( !eyeFireChargeScripts[ii].collected )
                    {
                        eyesAreCollected = false;
                    }
                }
                mouthFireChargeScripts[i].hide = !eyesAreCollected;
            }
        }
    }

    void UpdateLegs ()
    {
        for (int i = 0; i < legLines.Length; i++)
        {
            float legRow = (i == 0 || i == 1) ? 1f : -1f;
            float legDir = (i == 0 || i == 3 || i == 5) ? 1f : -1f;
            bool isLegTurn = (i == legTurn || (i == (legTurn + 2)) || (i == (legTurn + 3)));
            float hipW = myCore.myInfo.graphics.hipWidth;
            if ( i > 3 )
            {
                legRow = 0f;
            }

            Vector3 pStart, pEnd, pMid;

            pStart = graphicsTransform.position;
            pStart += (bodyUp * myCore.myInfo.graphics.legLength);
            pStart += (bodyRight * (hipW * legDir));
            pStart += (bodyForward * (legRow * myCore.myInfo.graphics.legSpaceDistance));

            if ( myCore.myInfo.graphics.flying )
            {
                pStart += (Vector3.up * myCore.myGraphics.flyOff);
            }

            pEnd = myTransform.position;
            pEnd += (bodyRight * (((hipW * 1.25f) * myCore.myInfo.graphics.legSpreadFactor) * legDir));
            pEnd += (bodyForward * (legRow * myCore.myInfo.graphics.legSpaceDistance));
            pEnd += (bodyForward * (legRow * myCore.myInfo.graphics.legRowEndSpreadFactor));

            if (myCore.myInfo.graphics.flying)
            {
                pEnd += (Vector3.up * (myCore.myGraphics.flyOff + myCore.myInfo.graphics.flyLegOff));
            }

            // walk animation
            if (myCore.myUnit.moving && !myCore.myInfo.graphics.flying && isLegTurn && myCore.curState != NpcCore.State.Hit)
            {
                Vector3 stepDir = bodyForward;
                stepDir.y = 0f;
                float stepLength = .125f * myCore.myInfo.graphics.stepFwdFactor;
                float stepHeight = .125f * myCore.myInfo.graphics.stepUpFactor;
                float stepP = BasicFunctions.ConvertRange((float)stepCounter, 0, (float)stepRate * 1f, 0f, 1f);
                stepP = Easing.Quintic.Out(stepP);

                pEnd += (stepDir * (Mathf.Sin((stepP - .5f) * Mathf.PI) * stepLength));
                pEnd += (Vector3.up * (Mathf.Cos((stepP - .5f) * Mathf.PI) * stepHeight));
            }

            // hit animation
            if (myCore.curState == NpcCore.State.Hit || myCore.curState == NpcCore.State.Stunned )
            {
                pEnd += (bodyForward * .125f);
                pEnd += (bodyUp * .075f);

                pEnd += (bodyRight * (legDir * .125f));
                float t0 = Time.time * (40f * myCore.myInfo.graphics.hitLegTimeFactor);
                float f0 = myCore.myInfo.graphics.hitLegDstFactor * (.5f + (myCore.forceDirCur.magnitude * .25f));
                float s0 = Mathf.Sin(t0) * f0;
                float s1 = Mathf.Cos(t0) * f0;
                pEnd += (bodyRight * (legDir * s0));
                pEnd += (bodyForward * (legDir * s1));
            }

            // alerted
            if (myCore.curState == NpcCore.State.Alerted)
            {
                pEnd += (bodyUp * (myCore.myInfo.graphics.alertedJumpHeight * alertedFac));
                pEnd += (bodyRight * ((myCore.myInfo.graphics.alertedLegSpread * legDir) * alertedFac));
            }

            // attack prepare
            if (myCore.curState == NpcCore.State.AttackPrepare)
            {
                pEnd += (bodyRight * ((myCore.myInfo.graphics.attackPrepareLegSpread * legDir) * attackPrepareFac));

                pEnd += (bodyForward * (myCore.myInfo.graphics.attackDoLegAnimationFwdOff * -legDir));
            }

            // attack do
            if (myCore.curState == NpcCore.State.AttackDo)
            {
                pEnd += (bodyRight * ((myCore.myInfo.graphics.attackDoLegSpread * legDir) * attackDoFac));
                pEnd += (bodyUp * ((myCore.myInfo.graphics.attackDoHeightOff * .5f) * attackDoFac));
                pEnd += (bodyForward * (myCore.myInfo.graphics.attackDoLegFwdOff * attackDoFac));

                pEnd += (bodyForward * (myCore.myInfo.graphics.attackDoLegAnimationFwdOff * legDir));
            }

            // block
            if ( myCore.curState == NpcCore.State.Block )
            {
                pEnd += (bodyRight * (((hipW * .5f) * myCore.myInfo.graphics.legSpreadFactor) * legDir));
            }

            // pin pEnd?
            float pEndDst = 1f;
            Vector3 pEnd0 = pEnd;
            pEnd0.y += pEndDst;
            Vector3 pEnd1 = pEnd;
            pEnd1.y -= pEndDst;
            RaycastHit pEndHit;
            if ( Physics.Linecast(pEnd0,pEnd1,out pEndHit,SetupManager.instance.groundLayerMask) )
            {
                Vector3 pEndHitPoint = pEndHit.point;
                if ( pEndHitPoint.y > pEnd.y )
                {
                    pEnd.y = pEndHitPoint.y + (myCore.myInfo.graphics.legWidth * .5f);
                }
            }

            pMid = BasicFunctions.LerpByDistance(pStart, pEnd, Vector3.Distance(pStart, pEnd) * .5f);
            pMid += (bodyForward * myCore.myInfo.graphics.kneeFwdOff);
            pMid += (bodyRight * (myCore.myInfo.graphics.kneeSideOff * legDir));
            pMid += (bodyUp * myCore.myInfo.graphics.kneeUpOff);

            // store 
            legPoints[i][0] = pStart;
            legPoints[i][1] = pMid;
            legPoints[i][2] = pEnd;

            // set
            legLines[i].SetPositions(legPoints[i]);

            // leg material
            legLines[i].material = (myCore.curState == NpcCore.State.Hit) ? VisualsDatabase.instance.npcHitMaterials[myCore.hitFlickerIndex] : legMatOriginal;
        }
    }

    void UpdateArms ()
    {
        for (int i = 0; i < armLines.Length; i++)
        {
            float armRow = (i == 0 || i == 1) ? 1f : -1f;
            float armDir = (i == 0 || i == 3) ? 1f : -1f;
            bool isArmTurn = (i == legTurn || (i == (legTurn + 2)));
            float shoulderW = myCore.myInfo.graphics.shoulderWidth;

            Vector3 pStart, pEnd, pMid;

            pStart = bodyTransform.position;
            pStart += (bodyUp * myCore.myInfo.graphics.shoulderHeight);
            pStart += (bodyRight * (shoulderW * armDir));
            //pStart += (bodyForward * (armRow * myCore.myInfo.graphics.armSpaceDistance));

            //if (myCore.myInfo.graphics.flying)
            //{
            //    pStart += (Vector3.up * myCore.myGraphics.flyOff);
            //}

            pEnd = pStart;
            pEnd += (bodyRight * (((shoulderW * 1.25f) * myCore.myInfo.graphics.armSpreadFactor) * armDir));
            pEnd += (bodyUp * -(myCore.myInfo.graphics.armLength));
            pEnd += (bodyUp * myCore.myInfo.graphics.armUpFac);
            //pEnd += (bodyForward * (armRow * myCore.myInfo.graphics.armSpaceDistance));

            //if (myCore.myInfo.graphics.flying)
            //{
            //    pStart += (Vector3.up * myCore.myGraphics.flyOff);
            //}

            // walk animation
            if (myCore.myUnit.moving && !myCore.myInfo.graphics.flying && isArmTurn && myCore.curState != NpcCore.State.Hit)
            {
                Vector3 stepDir = bodyForward;
                stepDir.y = 0f;
                float stepLength = -.125f * myCore.myInfo.graphics.stepFwdFactor;
                float stepHeight = .125f * myCore.myInfo.graphics.stepUpFactor;
                float stepP = BasicFunctions.ConvertRange((float)stepCounter, 0, (float)stepRate * 1f, 0f, 1f);
                stepP = Easing.Quintic.Out(stepP);

                pEnd += (stepDir * (Mathf.Sin((stepP - .5f) * Mathf.PI) * stepLength));
                pEnd += (Vector3.up * (Mathf.Cos((stepP - .5f) * Mathf.PI) * stepHeight));
            }

            // hit animation
            if (myCore.curState == NpcCore.State.Hit|| myCore.curState == NpcCore.State.Stunned)
            {
                pEnd += (bodyForward * .125f);
                pEnd += (bodyUp * .075f);

                pEnd += (bodyRight * (armDir * .125f));
                float t0 = Time.time * (40f * myCore.myInfo.graphics.hitArmTimeFactor);
                float f0 = myCore.myInfo.graphics.hitArmDstFactor * (.5f + (myCore.forceDirCur.magnitude * .25f));
                float s0 = Mathf.Sin(t0) * f0;
                float s1 = Mathf.Cos(t0) * f0;
                pEnd += (bodyRight * (armDir * s0));
                pEnd += (bodyForward * (armDir * s1));
            }

            // block animation
            if ( myCore.curState == NpcCore.State.Block )
            {
                pEnd += (bodyForward * .5f);
                pEnd += (bodyUp * .25f);
                pEnd += (bodyRight * (armDir * -.25f));
                if ( i == 1 )
                {
                    pEnd = armPoints[0][2];
                }
            }

            // alerted
            if (myCore.curState == NpcCore.State.Alerted)
            {
                pEnd += (bodyUp * (myCore.myInfo.graphics.alertedJumpHeight * alertedFac));
                pEnd += (bodyRight * ((myCore.myInfo.graphics.alertedLegSpread * armDir) * alertedFac));
            }

            // attack prepare
            if (myCore.curState == NpcCore.State.AttackPrepare)
            {
                pEnd += (bodyRight * ((myCore.myInfo.graphics.attackPrepareLegSpread * armDir) * attackPrepareFac));

                pEnd += (bodyForward * (myCore.myInfo.graphics.attackDoArmAnimationFwdOff * -armDir));
                pEnd += (bodyUp * (myCore.myInfo.graphics.attackDoArmAnimationUpOff * armDir));
                //pEnd += (bodyRight * (myCore.myInfo.graphics.attackDoArmAnimationSideOff * armDir));
            }

            // attack do
            if (myCore.curState == NpcCore.State.AttackDo)
            {
                pEnd += (bodyRight * ((myCore.myInfo.graphics.attackDoArmSpread * armDir) * attackDoFac));
                pEnd += (bodyUp * ((myCore.myInfo.graphics.attackDoHeightOff * .5f) * attackDoFac));
                pEnd += (bodyForward * ((myCore.myInfo.graphics.attackDoArmFwdOff) * attackDoFac));

                pEnd += (bodyForward * (myCore.myInfo.graphics.attackDoArmAnimationFwdOff * armDir));
                pEnd += (bodyUp * (myCore.myInfo.graphics.attackDoArmAnimationUpOff * -armDir));
                pEnd += (bodyRight * (myCore.myInfo.graphics.attackDoArmAnimationSideOff * -armDir));
            }

            pMid = BasicFunctions.LerpByDistance(pStart, pEnd, Vector3.Distance(pStart, pEnd) * .5f);
            pMid += (bodyForward * myCore.myInfo.graphics.elbowFwdOff);
            pMid += (bodyRight * (myCore.myInfo.graphics.elbowSideOff * armDir));
            if ( myCore.curState == NpcCore.State.Block )
            {
                pMid += (bodyRight * (myCore.myInfo.graphics.elbowSideOff * armDir));
            }

            // store 
            armPoints[i][0] = pStart;
            armPoints[i][1] = pMid;
            armPoints[i][2] = pEnd;

            // set
            armLines[i].SetPositions(armPoints[i]);

            // leg material
            armLines[i].material = (myCore.curState == NpcCore.State.Hit) ? VisualsDatabase.instance.npcHitMaterials[myCore.hitFlickerIndex] : armMatOriginal;
        }
    }

    void UpdateGraphics ()
    {
        // reset extra rotation
        bool overwriteRotation = false;
        Quaternion rotationOverwrite = Quaternion.identity;

        bool graphicsFaceDoOverride = false;
        Vector3 graphicsFaceOverridePoint = Vector3.zero;

        bool graphicsFacePlayer = false;
        Vector3 extraRot = Vector3.zero;

        // define position & rotation per state
        switch ( myCore.curState )
        {
            default:
                graphicsTransform.localPosition = Vector3.zero;

                graphicsLocalRotTarget = graphicsLocalRotOriginal;
                graphicsLocalRotCur = graphicsLocalRotTarget;
                break;
            case NpcCore.State.Hit:
                {
                    graphicsTransform.localPosition = new Vector3(0f, .1f, 0f);

                    if (!myCore.hitButImmune)
                    {
                        graphicsLocalRotTarget.y += (myCore.forceDirCur.magnitude * 400f) * Time.deltaTime;

                        float t0 = Time.time * 15f;
                        float f0 = 20f * (.5f + (myCore.forceDirCur.magnitude * .25f));
                        float s0 = Mathf.Sin(t0 + timeOffset) * f0;
                        float s1 = Mathf.Cos(t0 + timeOffset) * f0;
                        extraRot.x = s0;
                        extraRot.z = s1;
                    }
                    else
                    {
                        graphicsFacePlayer = true;
                    }
                }
                break;
            case NpcCore.State.Alerted:
                float alertedP = BasicFunctions.ConvertRange((float)myCore.alertedCounter,0f,(float)myCore.alertedDur,0f,1f);
                alertedFac = Easing.Quintic.InOut(Mathf.Sin(alertedP * Mathf.PI));
                graphicsTransform.localPosition = Vector3.Lerp(graphicsTransform.localPosition,new Vector3(0f, myCore.myInfo.graphics.alertedJumpHeight * alertedFac, 0f),10f * Time.deltaTime);

                graphicsFacePlayer = true;
                break;
            case NpcCore.State.Chase:
                graphicsTransform.localPosition = Vector3.zero;

                graphicsFacePlayer = true;
                break;

            case NpcCore.State.LightCannon:
                graphicsTransform.localPosition = Vector3.zero;

                graphicsFacePlayer = false;

                graphicsFaceDoOverride = true;
                graphicsFaceOverridePoint = myCore.cannonScriptTarget.myTransform.position;
                graphicsFaceOverridePoint.y = myCore.myTransform.position.y;
                
                break;

            case NpcCore.State.AttackPrepare:
                {
                    float attackPrepareP = BasicFunctions.ConvertRange((float)myCore.attackPrepareCounter, 0f, (float)myCore.curAttackData.attackPrepareDur, 0f, 1f);
                    attackPrepareFacReal = attackPrepareP;
                    attackPrepareFac = Easing.Quintic.InOut(Mathf.Sin(attackPrepareP * Mathf.PI));

                    Transform shakeRefTr = graphicsTransform;
                    Vector3 attackPrepareShakeOff = Vector3.zero;
                    float t0 = Time.time;
                    float sX = Mathf.Sin(t0 * myCore.myInfo.graphics.attackPrepareShakeTime.x) * myCore.myInfo.graphics.attackPrepareShakeDst.x;
                    float sY = Mathf.Sin(t0 * myCore.myInfo.graphics.attackPrepareShakeTime.y) * myCore.myInfo.graphics.attackPrepareShakeDst.y;
                    float sZ = Mathf.Sin(t0 * myCore.myInfo.graphics.attackPrepareShakeTime.z) * myCore.myInfo.graphics.attackPrepareShakeDst.z;
                    attackPrepareShakeOff += (shakeRefTr.right * sX);
                    attackPrepareShakeOff += (shakeRefTr.up * sY);
                    attackPrepareShakeOff += (shakeRefTr.forward * sZ);

                    graphicsTransform.localPosition = Vector3.Lerp(graphicsTransform.localPosition, new Vector3(0f, myCore.myInfo.graphics.attackPrepareHeightOff * attackPrepareFac, 0f) + attackPrepareShakeOff, 10f * Time.deltaTime);

                    graphicsFacePlayer = true;
                    if (myCore.overrideAttackPoint)
                    {
                        graphicsFaceDoOverride = true;
                        graphicsFaceOverridePoint = myCore.attackPointOverride;
                    }

                    if (myCore.curAttackData.facePlayer)
                    {
                        overwriteRotation = false;
                        graphicsFaceDoOverride = false;
                        graphicsFacePlayer = true;
                    }
                }
                break;

            case NpcCore.State.AttackDo:
                float attackDoP = BasicFunctions.ConvertRange((float)myCore.attackDoCounter, 0f, (float)myCore.curAttackData.attackDoDur * .25f, 0f, 1f);
                attackDoFac = Easing.Quintic.InOut(Mathf.Sin(attackDoP * Mathf.PI));
                graphicsTransform.localPosition = Vector3.Lerp(graphicsTransform.localPosition, new Vector3(0f, myCore.myInfo.graphics.attackDoHeightOff * attackDoFac, 0f), 10f * Time.deltaTime);

                overwriteRotation = true;
                rotationOverwrite = attackPrepareRot;
                if (myCore.overrideAttackPoint)
                {
                    graphicsFaceDoOverride = true;
                    graphicsFaceOverridePoint = myCore.attackPointOverride;
                }

                if ( myCore.curAttackData.facePlayer )
                {
                    overwriteRotation = false;
                    graphicsFaceDoOverride = false;
                    graphicsFacePlayer = true;
                }
                break;

            case NpcCore.State.Block:
                graphicsFacePlayer = true;

                graphicsTransform.localPosition = Vector3.zero;

                graphicsLocalRotTarget = graphicsLocalRotOriginal;
                graphicsLocalRotCur = graphicsLocalRotTarget;
                break;

            case NpcCore.State.Stunned:
                graphicsFacePlayer = true;

                graphicsTransform.localPosition = Vector3.zero;

                graphicsLocalRotTarget = graphicsLocalRotOriginal;
                graphicsLocalRotCur = graphicsLocalRotTarget;
                break;

            case NpcCore.State.Vulnerable:
                {
                    graphicsTransform.localPosition = new Vector3(0f, .1f, 0f);

                    float t0 = Time.time * 15f;
                    float f0 = 30f * (.5f + (myCore.forceDirCur.magnitude * .25f));
                    float s0 = Mathf.Sin(t0 + timeOffset) * f0;
                    float s1 = Mathf.Cos(t0 + timeOffset) * f0;
                    extraRot.x = s0;
                    extraRot.z = s1;

                    graphicsFacePlayer = true;
                }
                break;

            case NpcCore.State.Sleeping:
                graphicsTransform.localPosition = Vector3.zero;//new Vector3(0f,.1f,0f);

                overwriteRotation = true;
                rotationOverwrite = Quaternion.Euler(-90f,145f,0f);
                break;

            case NpcCore.State.WakeUp:
                {
                    float t0 = Time.time * 40f;
                    float f0 = .175f;
                    float s0 = Mathf.Sin(t0) * f0;

                    graphicsTransform.localPosition = new Vector3(s0,0f,0f);

                    graphicsFacePlayer = true;
                }
                break;

            case NpcCore.State.Laugh:
                {
                    float t0 = Time.time * 20f;
                    float f0 = .1f;
                    float s0 = Mathf.Sin(t0) * f0;

                    graphicsTransform.localPosition = new Vector3(0, s0, 0f);

                    graphicsFacePlayer = true;
                }
                break;

            case NpcCore.State.CollectFire:
                graphicsFacePlayer = true;

                graphicsTransform.localPosition = Vector3.zero;

                graphicsLocalRotTarget = graphicsLocalRotOriginal;
                graphicsLocalRotCur = graphicsLocalRotTarget;
                break;

            case NpcCore.State.EnterNextStage:
                graphicsFacePlayer = true;

                graphicsTransform.localPosition = Vector3.zero;

                graphicsLocalRotTarget = graphicsLocalRotOriginal;
                graphicsLocalRotCur = graphicsLocalRotTarget;
                break;
        }

        // lerp & set rotation
        if (!graphicsFaceDoOverride)
        {
            if (!overwriteRotation)
            {
                if (!graphicsFacePlayer)
                {
                    graphicsLocalRotCur = Vector3.Lerp(graphicsLocalRotCur, graphicsLocalRotTarget, 20f * Time.deltaTime);
                    graphicsTransform.localRotation = Quaternion.Euler(graphicsLocalRotCur) * Quaternion.Euler(extraRot);
                }
                else
                {
                    GraphicsFacePosition(GameManager.instance.playerFirstPersonDrifter.myTransform.position,extraRot);
                }
            }
            else
            {
                graphicsTransform.rotation = rotationOverwrite;
            }
        }
        else
        {
            GraphicsFacePosition(graphicsFaceOverridePoint,extraRot);
        }

        // store attack prepare rotation?
        if ( myCore.curState == NpcCore.State.AttackPrepare )
        {
            attackPrepareRot = graphicsTransform.rotation;
        }
    }

    public void GraphicsFacePosition ( Vector3 _p, Vector3 _extra )
    {
        Vector3 r0 = myTransform.position;
        Vector3 r1 = _p;
        Vector3 r2 = (r1 - r0);
        r2.y = 0f;

        if (r2.magnitude > .025f)
        {
            Quaternion targetRot = Quaternion.LookRotation(r2.normalized) * Quaternion.Euler(_extra);
            graphicsTransform.rotation = targetRot;
        }
    }

    public void SetScale ( float _fac )
    {
        if ( graphicsTransform != null )
        {
            graphicsTransform.localScale = (graphicsSclOriginal * _fac);
        }
    }

    void UpdateBody ()
    {
        if ( bodyTransform != null )
        {
            Vector3 bodyP = myCore.myInfo.graphics.bodyOffset;

            switch ( myCore.curState )
            {
                default:
                    bodyTransform.localRotation = Quaternion.identity;
                    break;

                case NpcCore.State.Stunned:
                    bodyTransform.localRotation = Quaternion.Euler(myCore.myInfo.graphics.bodyHitFwdRot * .5f, 0f, 0f);
                    break;

                case NpcCore.State.Vulnerable:
                    bodyTransform.localRotation = Quaternion.Euler(myCore.myInfo.graphics.bodyHitFwdRot * .75f, 0f, 0f);
                    break;

                case NpcCore.State.Hit:
                    bodyTransform.localRotation = Quaternion.Euler(myCore.myInfo.graphics.bodyHitFwdRot, 0f, 0f);
                    break;

                case NpcCore.State.Alerted:
                    bodyTransform.localRotation = Quaternion.Euler(myCore.myInfo.graphics.bodyAlertedFwdRot, 0f, 0f);
                    break;

                case NpcCore.State.AttackPrepare:
                    bodyTransform.localRotation = Quaternion.Euler(myCore.myInfo.graphics.bodyAttackPrepareFwdRot, 0f, 0f);
                    break;

                case NpcCore.State.AttackDo:
                    bodyTransform.localRotation = Quaternion.Euler(myCore.myInfo.graphics.bodyAttackDoFwdRot, 0f, 0f);
                    break;

                case NpcCore.State.Block:
                    bodyTransform.localRotation = Quaternion.Euler(myCore.myInfo.graphics.bodyBlockFwdRot, 0f, 0f);
                    break;
            }

            // flying?
            if ( myCore.myInfo.graphics.flying && myCore.curState != NpcCore.State.Sleeping )
            {
                float t0 = (Time.time * myCore.myInfo.graphics.flyTimeFactor);
                float f0 = myCore.myInfo.graphics.flyDstFactor;
                float s0 = Mathf.Sin(t0) * f0;
                flyOff = (flyHeightUse + s0);
                bodyP.y += flyOff;
            }

            bodyTransform.localPosition = bodyP;

            // scaling
            bodySclTarget = bodySclOriginal;
            if ( myCore.myInfo.graphics.bodySlugAnimation )
            {
                if (myCore.myUnit.moving)
                {
                    float t0 = (Time.time * myCore.myInfo.graphics.bodySlugTimeFactor);
                    float f0 = myCore.myInfo.graphics.bodySlugDstFactor;
                    float s0 = Mathf.Sin(t0) * f0;
                    bodySclTarget.z += s0;
                }
            }
            bodySclCur = bodySclTarget;
            bodyTransform.localScale = bodySclCur;
        }

        for ( int i = 0; i < bodyMeshRenderers.Length; i ++ )
        {
            bodyMeshRenderers[i].material = ( myCore.curState == NpcCore.State.Hit ) ? VisualsDatabase.instance.npcHitMaterials[myCore.hitFlickerIndex] : bodyMatsOriginal[i];
        }
    }

    void UpdateHead ()
    {
        if ( headTransform != null )
        {
            Vector3 headP = myCore.myInfo.graphics.headOffset;

            headTransform.localPosition = headP;

            Vector3 headExtraRot = Vector3.zero;
            bool overrideHeadRotation = false;
            float timeFac = 10f;

            switch ( myCore.curState )
            {
                case NpcCore.State.Hit:
                    overrideHeadRotation = true;
                    if (overrideHeadRotation)
                    {
                        float t0 = (Time.time * (20f * myCore.myInfo.graphics.headAnimTimeFactor));
                        float f0 = (20f * (.5f + (myCore.forceDirCur.magnitude * .25f))) * myCore.myInfo.graphics.headAnimDstFactor;
                        float s0 = Mathf.Sin(t0 + timeOffset) * f0;
                        float s1 = Mathf.Cos(t0 + timeOffset) * f0;
                        headTransform.localRotation = Quaternion.Euler(s0, 0f, s1);
                    }
                    break;

                case NpcCore.State.Vulnerable:
                    overrideHeadRotation = true;
                    if (overrideHeadRotation)
                    {
                        float t0 = (Time.time * (20f * myCore.myInfo.graphics.headAnimTimeFactor));
                        float f0 = (20f * myCore.myInfo.graphics.headAnimDstFactor);
                        float s0 = Mathf.Sin(t0 + timeOffset) * f0;
                        float s1 = Mathf.Cos(t0 + timeOffset) * f0;
                        headTransform.localRotation = Quaternion.Euler(s0, 0f, s1);
                    }
                    break;

                case NpcCore.State.Stunned:
                    overrideHeadRotation = true;
                    if (overrideHeadRotation)
                    {
                        float t0 = (Time.time * (20f * myCore.myInfo.graphics.headAnimTimeFactor));
                        float f0 = (20f * (.5f + (myCore.forceDirCur.magnitude * .25f))) * myCore.myInfo.graphics.headAnimDstFactor;
                        float s0 = Mathf.Sin(t0 + timeOffset) * f0;
                        float s1 = Mathf.Cos(t0 + timeOffset) * f0;
                        headTransform.localRotation = Quaternion.Euler(s0, 0f, s1);
                    }
                    break;

                case NpcCore.State.Chase:
                    timeFac = 20f;
                    break;

                case NpcCore.State.AttackPrepare:
                    headExtraRot.x = -10f;
                    break;

                case NpcCore.State.AttackDo:
                    overrideHeadRotation = true;

                    headTransform.localRotation = Quaternion.Euler(0f,0f,0f);
                    break;
            }

            if (!overrideHeadRotation)
            {
                float t0 = (Time.time * (timeFac * myCore.myInfo.graphics.headAnimTimeFactor));
                float f0 = (10f * myCore.myInfo.graphics.headAnimDstFactor);
                float s0 = Mathf.Sin(t0 + timeOffset) * f0;
                headTransform.localRotation = Quaternion.Euler(0f, s0, 0f) * Quaternion.Euler(headExtraRot);
            }
        }

        for ( int i = 0; i < headMeshRenderers.Length; i ++ )
        {
            headMeshRenderers[i].material = ( myCore.curState == NpcCore.State.Hit ) ? VisualsDatabase.instance.npcHitMaterials[myCore.hitFlickerIndex] : headMatsOriginal[i];
        }
    }

    void UpdateMouth ()
    {
        if ( mouthTransforms != null && mouthTransforms.Length > 0 )
        {
            for ( int i = 0; i < mouthTransforms.Length; i ++ )
            {
                float mouthDir = (i == 0) ? 1f : -1f;

                mouthRotTarget[i] = mouthRotOriginal[i];

                switch (myCore.curState)
                {
                    case NpcCore.State.Hit:
                        mouthRotTarget[i] += new Vector3(myCore.myInfo.graphics.mouthHitSpreadFac * mouthDir,0f,0f);
                        break;
                    case NpcCore.State.AttackPrepare:
                        if ( myCore.curAttackData.openMouthWhenPreparing )
                        {
                            mouthRotTarget[i] += new Vector3((myCore.myInfo.graphics.mouthAttackPrepareSpreadExtra * mouthDir) * attackPrepareFacReal, 0f, 0f);
                        }
                        break;
                    case NpcCore.State.AttackDo:
                        float mouthP = attackDoFac;
                        if ( myCore.curAttackData.rangedAttackType == Npc.AttackData.RangedAttackType.Laser )
                        {
                            mouthP = 1f;
                        }
                        mouthRotTarget[i] += new Vector3((myCore.myInfo.graphics.mouthHitSpreadFac * mouthDir) * mouthP, 0f, 0f);
                        break;
                    case NpcCore.State.Stunned:
                        mouthRotTarget[i] += new Vector3(myCore.myInfo.graphics.mouthHitSpreadFac * mouthDir, 0f, 0f);
                        break;
                    case NpcCore.State.Block:
                        mouthRotTarget[i] += new Vector3(myCore.myInfo.graphics.mouthBlockSpreadFac * mouthDir, 0f, 0f);
                        break;

                    case NpcCore.State.Vulnerable:
                        float t0 = (Time.time * 20f);
                        float f0 = 30f;
                        float s0 = Mathf.Sin(t0) * f0;
                        mouthRotTarget[i] += new Vector3((myCore.myInfo.graphics.mouthHitSpreadFac + s0) * mouthDir, 0f, 0f);
                        break;

                }

                mouthRotCur[i] = mouthRotTarget[i];
                mouthTransforms[i].localRotation = Quaternion.Euler(mouthRotCur[i]);
            }
        }
    }

    void UpdateTail ()
    {
        if ( tailLine != null )
        {
            Transform trUse = ( myCore.myInfo.graphics.hasBody ) ? bodyTransform : graphicsTransform;
            int tailSegments = tailPoints.Length;
            float tailSegmentsFloat = (float)(tailSegments);
            Vector3 tailRoot = trUse.position;
            Vector3 tailOff = myCore.myInfo.graphics.tailOffset;
            tailRoot += (trUse.right * tailOff.x);
            tailRoot += (trUse.up * tailOff.y);
            tailRoot += (trUse.forward * tailOff.z);
            for (int i = 0; i < tailSegments; i++)
            {
                float iFloat = (float)(i);
                float timeFac = 10f;

                Vector3 tailDir = trUse.forward * -(myCore.myInfo.graphics.tailLength / tailSegmentsFloat);
                Vector3 tailP = tailRoot + (tailDir * iFloat);
                tailP += (Vector3.up * -(myCore.myInfo.graphics.tailVerAdd * iFloat));

                if (myCore.curState == NpcCore.State.Hit)
                {
                    tailP += (bodyTransform.up * (.125f * iFloat));
                    timeFac = 20f;
                }
                if (myCore.curState == NpcCore.State.Stunned)
                {
                    tailP += (bodyTransform.up * (.125f * iFloat));
                    timeFac = 20f;
                }
                if ( myCore.curState == NpcCore.State.Chase )
                {
                    tailP += (bodyTransform.up * (.175f * iFloat));
                    timeFac = 20f;
                }

                float t0 = Time.time * timeFac;
                float f0 = .1f * iFloat;
                float s0 = Mathf.Sin(t0 + (.5f * iFloat) + timeOffset) * f0;
                tailP += bodyTransform.right * s0;

                tailPoints[i] = tailP;
            }
            tailLine.SetPositions(tailPoints);

            tailLine.material = (myCore.curState == NpcCore.State.Hit) ? VisualsDatabase.instance.npcHitMaterials[myCore.hitFlickerIndex] : tailMatOriginal;
        }
    }

    void UpdateFeet ()
    {
        for ( int i = 0; i < feetTransforms.Length; i ++ )
        {
            feetTransforms[i].position = legPoints[i][2];
            feetTransforms[i].rotation = Quaternion.LookRotation(graphicsForward);

            feetMeshRenderers[i].material = (myCore.curState == NpcCore.State.Hit) ? VisualsDatabase.instance.npcHitMaterials[myCore.hitFlickerIndex] : feetMatOriginal;
        }
    }

    void UpdateWings ()
    {
        for (int i = 0; i < wingTransforms.Length; i++)
        {
            float wingDir = (i == 0) ? 1f : -1f;
            float wingSideDst = myCore.myInfo.graphics.wingSideDst;
            float wingUpDst = myCore.myInfo.graphics.wingUpDst;
            float wingFwdDst = myCore.myInfo.graphics.wingFwdDst;

            // pos
            Vector3 wingP = bodyTransform.position;
            wingP += (bodyTransform.right * (wingSideDst * wingDir));
            wingP += (bodyTransform.up * wingUpDst);
            wingP += (bodyTransform.forward * wingFwdDst);
            wingTransforms[i].position = wingP;

            // rot
            float t0 = (Time.time * myCore.myInfo.graphics.wingTimeFactor);
            float f0 = myCore.myInfo.graphics.wingDstFactor;
            float s0 = Mathf.Sin(t0) * f0;
            Quaternion rotOff = Quaternion.Euler(s0,0f,0f);

            Quaternion wingR = Quaternion.LookRotation(bodyTransform.forward) * Quaternion.Euler(0f,90f * wingDir,0f) * Quaternion.Euler(myCore.myInfo.graphics.wingRotOff * wingDir) * rotOff;
            wingTransforms[i].rotation = wingR;

            // material
            wingMeshRenderers[i].material = (myCore.curState == NpcCore.State.Hit) ? VisualsDatabase.instance.npcHitMaterials[myCore.hitFlickerIndex] : wingMatOriginal;
        }
    }

    void UpdateHand()
    {
        for (int i = 0; i < handTransforms.Length; i++)
        {
            float handDir = (i == 0) ? 1f : -1f;

            Vector3 p0 = armPoints[i][1];
            Vector3 p1 = armPoints[i][2];
            Vector3 p2 = (p1 - p0).normalized;

            handTransforms[i].position = p1;

            Quaternion handRot = Quaternion.LookRotation(p2) * Quaternion.Euler(0f,0f,90f);
            if ( i == 0 )
            {
                handRot *= Quaternion.Euler(0f,0f,180f);
            }
            handTransforms[i].rotation = handRot;

            handMeshRenderers[i].material = (myCore.curState == NpcCore.State.Hit) ? VisualsDatabase.instance.npcHitMaterials[myCore.hitFlickerIndex] : handMatOriginal;
        }

        // equipment?
        if (myCore.myInfo.graphics.hasEquipment)
        {
            for (int i = 0; i < equipmentTransforms.Length; i++)
            {
                float armDir = (i == 0) ? 1f : -1f;

                Vector3 p0 = armPoints[i][1];
                Vector3 p1 = armPoints[i][2];
                Vector3 p2 = (p1 - p0).normalized;

                Vector3 equipmentP = handTransforms[i].position;
                equipmentP += (handTransforms[i].up * myCore.myInfo.graphics.equipmentHoldOffsets[i].y);
                equipmentP += (handTransforms[i].forward * myCore.myInfo.graphics.equipmentHoldOffsets[i].z);
                equipmentP += (handTransforms[i].right * myCore.myInfo.graphics.equipmentHoldOffsets[i].x);

                //equipmentP += (handTransforms[i].up * -.05f);
                //equipmentP += (handTransforms[i].forward * .1f);

                Quaternion equipmentR = Quaternion.LookRotation(p2) * Quaternion.Euler(0f,90f,0f); //Quaternion.LookRotation(handTransforms[i].up,-handTransforms[i].right);
                if ( myCore.curState == NpcCore.State.AttackDo )
                {
                    //equipmentR = Quaternion.LookRotation(bodyForward);
                }
                if ( myCore.curState == NpcCore.State.Block )
                {
                    float rOff = -(70f * armDir);
                    equipmentR *= Quaternion.Euler(rOff,0f,0f);
                    equipmentP += (handTransforms[i].right * (.1f * armDir));
                    equipmentP += (handTransforms[i].up * .05f);
                }
                equipmentTransforms[i].position = equipmentP;
                equipmentTransforms[i].rotation = equipmentR;
            }
        }
    }

    public void Clear ()
    {
        
    }
}
