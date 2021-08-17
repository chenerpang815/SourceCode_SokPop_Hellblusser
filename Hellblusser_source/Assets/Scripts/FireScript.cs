using UnityEngine;

public class FireScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // graphics
    [Header("graphics")]
    public Transform graphics;

    // npc
    [Header("npc")]
    public NpcCore npcConnectedTo;

    // settings
    public Transform transformFollow;

    public enum FireType { Normal, Blue };
    public FireType myFireType;

    public Material normalFireMat;
    public Material blueFireMat;

    // scale
    [HideInInspector] public float scaleMultiplier;
    Vector3 sclOriginal;
    Vector3 sclTarget;
    Vector3 sclCur;
    Vector3 sclAdd;

    public bool canBurnPlayer;

    public FlameableScript myFlameableScript;
    
    public MeshRenderer myMeshRenderer;
    public Material normalMat, highlightMat;
    int highlightDur, highlightCounter;

    public bool autoClear;
    int startAutoClearDur, startAutoClearCounter;
    float yFactor;

    [HideInInspector] public bool onStart;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // scale
        sclOriginal = graphics.localScale * scaleMultiplier;
        sclOriginal.x *= TommieRandom.instance.RandomRange(.75f,1.75f);
        sclOriginal.y *= TommieRandom.instance.RandomRange(.75f,1.75f);

        sclTarget = sclOriginal;

        Vector3 sclStart = sclOriginal;
        sclStart.x *= 2.75f;
        sclStart.y *= .125f;
        sclCur = sclStart;
        graphics.localScale = sclCur;

        // create decal
        DecalManager.instance.AddDecal(myTransform.position,sclOriginal.x * .375f);

        // auto clear
        startAutoClearDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(600f,800f));
        startAutoClearCounter = 0;
        yFactor = 1f;

        // burn flameable objects?
        if (GameManager.instance != null && GameManager.instance.allFlameableObjects != null)
        {
            for (int i = 0; i < GameManager.instance.allFlameableObjects.Count; i++)
            {
                FlameableScript curFlameableScript = GameManager.instance.allFlameableObjects[i];
                if (curFlameableScript != null)
                {
                    Vector3 p0 = myTransform.position;
                    Vector3 p1 = curFlameableScript.myTransform.position;
                    float d0 = Vector3.Distance(p0, p1);
                    if (d0 <= curFlameableScript.radius)
                    {
                        curFlameableScript.AddFlameableProgress(1f, false, true, myFireType);
                        if (autoClear && curFlameableScript.curState == FlameableScript.State.Active)
                        {
                            Clear();
                        }
                    }
                }
            }
        }

        // highlight
        highlightDur = 2;
        highlightCounter = highlightDur;

        // audio
        if (!onStart)
        {
            AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireStartClips), .9f,1.2f, .3f, .325f);
        }

        // store
        Store();
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze)
        {
            if ( transformFollow != null )
            {
                myTransform.position = transformFollow.position;
            }

            //sclCur = Vector3.Lerp(sclCur,sclTarget,5f * Time.deltaTime);
            sclAdd = BasicFunctions.SpringVector(sclAdd, sclCur, sclTarget, .01f, .25f, .5f, .9f);
            sclCur += sclAdd;

            float t0 = Time.time * 20f;
            float f0 = .01f;
            float s0 = Mathf.Sin(t0) * f0;
            sclTarget.y += s0;

            if (autoClear)
            {
                if (startAutoClearCounter < startAutoClearDur)
                {
                    startAutoClearCounter++;
                }
                else
                {
                    yFactor = .95f;
                    if (sclTarget.y <= .125f)
                    {
                        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position, Quaternion.identity, .5f);
                        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[1], myTransform.position, Quaternion.identity, 1f);
                        Clear();
                    }
                }
            }

            sclTarget *= yFactor;

            graphics.localScale = sclCur;

            // highlight?
            if ( myMeshRenderer != null )
            {
                if ( highlightCounter < highlightDur )
                {
                    highlightCounter++;
                }
                myMeshRenderer.material = (highlightCounter >= highlightDur) ? normalMat : highlightMat;
            }
        }
    }

    public void Store ()
    {
        if (GameManager.instance != null && GameManager.instance.fireScripts != null)
        {
            GameManager.instance.fireScripts.Add(this);
        }
    }

    public void Highlight ()
    {
        highlightCounter = 0;
    }

    public void Douse ()
    {
        // particles
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position, Quaternion.identity, .5f);
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[1], myTransform.position, Quaternion.identity, 1f);

        // doei druif
        Clear();

        // log
        //Debug.Log("vuurtje doof || " + Time.time.ToString());
    }

    public void Collect ( bool _byPlayer, NpcCore _npcBy )
    {
        SetupManager.instance.curProgressData.persistentData.sawFirePopup = true;

        // deal damage to connected npc?
        if ( npcConnectedTo != null && npcConnectedTo.myInfo.stats.healthIsFire )
        {
            npcConnectedTo.DealDamage(1,Npc.AttackData.DamageType.Special);

            Vector3 p0 = npcConnectedTo.myTransform.position;
            Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
            p1.y = p0.y;
            Vector3 d2 = (p1 - p0).normalized;
            npcConnectedTo.InitGetHit(d2,-10f,npcConnectedTo.myInfo.stats.hitDur,true);
        }

        // particles
        float whiteOrbScl = ( _npcBy == null ) ? .5f : .875f;

        // fire burst blessing?
        if (npcConnectedTo == null)
        {
            if (SetupManager.instance != null && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.FireBurst))
            {
                PrefabManager.instance.SpawnDamageDeal(myTransform.position, 1.5f, 1, Npc.AttackData.DamageType.Magic, 10, HandManager.instance.myTransform, .325f, true, DamageDeal.Target.AI, null, false, false);
                whiteOrbScl = 1.5f;
            }
        }

        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position, Quaternion.identity, whiteOrbScl);
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[1], myTransform.position, Quaternion.identity, 1f);

        // had flameable?
        if ( myFlameableScript != null )
        {
            myFlameableScript.myFireScript = null;
        }

        // collect!
        GameObject fireCollectO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.fireCollectPrefab,myTransform.position,Quaternion.identity,1f);
        FireCollectScript fireCollectScript = fireCollectO.GetComponent<FireCollectScript>();
        if ( fireCollectScript != null )
        {
            fireCollectScript.targetTransform = (_byPlayer) ? GameManager.instance.playerFirstPersonDrifter.myTransform : _npcBy.fireCollectPointTransform;
            fireCollectScript.npcCollectedBy = _npcBy;
        }

        // log
        //Debug.Log("fire collected! by: " + _npcBy + " || by player: " + _byPlayer + " || " + Time.time.ToString());

        // audio
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireStartClips), 1.4f, 1.6f, .1f, .125f);

        // freeze
        if (_byPlayer)
        {
            SetupManager.instance.SetFreeze(4);
        }

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

        // doei druif
        Clear();

        // log
        //Debug.Log("geef me die vuurtje || " + Time.time.ToString());
    }

    public void SetFireType ( FireType _type )
    {
        myFireType = _type;

        // material
        switch (myFireType)
        {
            case FireType.Normal:
                normalMat = normalFireMat;
                break;
            case FireType.Blue:
                normalMat = blueFireMat;
                autoClear = false;

                sclOriginal.x *= TommieRandom.instance.RandomRange(1.25f, 1.375f);
                sclOriginal.y *= TommieRandom.instance.RandomRange(1.25f, 1.375f);
                break;
        }
    }

    public void Clear ()
    {
        // clear reference
        if (GameManager.instance != null && GameManager.instance.fireScripts != null)
        {
            if ( GameManager.instance.fireScripts.Contains(this) )
            {
                GameManager.instance.fireScripts.Remove(this);
            }
        }

        // audio
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireStopClips), .6f, .9f, .075f, .1f);

        // doeg
        Destroy(myGameObject);
    }
}
