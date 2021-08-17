using UnityEngine;

public class FlameableScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // references
    public FireScript myFireScript;

    // settings
    [Header("settings")]
    public float burnThreshold;
    public float radius;
    public Vector3 fireScale;
    public Vector3 fireOffset;
    public bool preventFireOnStart;
    public bool forceFireOnStart;
    public bool canBeRekindled;
    public bool moveableFire;
    public bool fireIsChild;
    public bool overrideFakkel;
    public float fireHitBoxSizeExtra;
    public NpcCore npcConnectedTo;
    [HideInInspector] public bool wasConnectedToNpc;

    public bool preventCreatedFireFromBurningPlayer;
    public bool autoCreateFire;
    public int autoCreateFireDur;
    int autoCreateFireCounter;
    public FireScript.FireType autoCreateFireType;
    public FireScript.FireType forceFireOnStartType;

    // generated
    public bool generationInit;
    bool generated;

    // state
    public enum State { Inactive, Active };
    public State curState;
    [HideInInspector] public float flameableProgress;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;

        generated = false;
        if ( !generationInit )
        {
            Init();
        }
    }

    void Init ()
    {
        SetState(State.Inactive);

        // connected to npc?
        if (npcConnectedTo != null)
        {
            wasConnectedToNpc = true;
        }

        // boem in de fik
        if (!preventFireOnStart)
        {
            SetupManager.instance.CheckFireChance();
            if (TommieRandom.instance.GenerationRandomValue(1f, "flameable start") <= SetupManager.instance.fireSpawnChance)
            {
                AddFlameableProgress(100f, true, false, FireScript.FireType.Normal);
            }
        }

        // force fire on start?
        if (forceFireOnStart)
        {
            AddFlameableProgress(100f, true, false, forceFireOnStartType);
        }

        // store
        Store();

        // done
        generated = true;
    }

    public void AddFlameableProgress ( float _amount, bool _onStart, bool _allowRekindle, FireScript.FireType _fireType )
    {
        if (curState == State.Inactive || (canBeRekindled && _allowRekindle))
        {
            flameableProgress += _amount;
            if (flameableProgress >= burnThreshold && myFireScript == null)
            {
                SetState(State.Active);

                CreateFireObject(_onStart,_fireType);
            }
        }
    }

    public void CreateFireObject ( bool _onStart, FireScript.FireType _fireType )
    {
        // create fire object
        Vector3 fireP = myTransform.position;
        fireP += fireOffset;
        GameObject fireImpactO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.firePrefab, fireP, Quaternion.identity, 1f);
        Transform fireImpactTr = fireImpactO.transform;
        fireImpactTr.localScale = fireScale;
        FireScript fireImpactScript = fireImpactO.GetComponent<FireScript>();
        if (fireImpactScript != null)
        {
            fireImpactScript.npcConnectedTo = npcConnectedTo;
            fireImpactScript.scaleMultiplier = 1f;
            fireImpactScript.autoClear = false;
            fireImpactScript.onStart = _onStart;
            fireImpactScript.myFlameableScript = this;
            fireImpactScript.canBurnPlayer = !preventCreatedFireFromBurningPlayer;

            if (fireHitBoxSizeExtra > 0f)
            {
                fireImpactO.GetComponent<BoxCollider>().size *= fireHitBoxSizeExtra;
            }

            if (moveableFire)
            {
                fireImpactScript.transformFollow = myTransform;
            }

            fireImpactScript.SetFireType(_fireType);

            myFireScript = fireImpactScript;
        }

        if ( fireIsChild )
        {
            fireImpactTr.parent = myTransform;
        }

        // create decal
        DecalManager.instance.AddDecal(myTransform.position, fireScale.x * .5f);
    }

    void Update ()
    {
        if (!generated)
        {
            if ( (LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.generatedLevel) || (SetupManager.instance.curGameState == SetupManager.GameState.Rest || SetupManager.instance.curGameState == SetupManager.GameState.Shop || SetupManager.instance.curGameState == SetupManager.GameState.Outro) )
            {
                Init();
            }
        }
        else
        {
            if (!SetupManager.instance.inFreeze && !SetupManager.instance.paused && (SetupManager.instance.runDataRead.playerFireCur <= 0 || overrideFakkel))
            {
                if (autoCreateFire && myFireScript == null)
                {
                    if (autoCreateFireCounter < autoCreateFireDur)
                    {
                        autoCreateFireCounter++;
                    }
                    else
                    {
                        CreateFireObject(false, autoCreateFireType);
                        autoCreateFireCounter = 0;
                    }
                }

                // clear because connected but npc doesn't exist anymore :'(
                if (wasConnectedToNpc && npcConnectedTo == null)
                {
                    Clear();
                }
            }
        }
    }

    public void SetState ( State _to )
    {
        curState = _to;
    }

    public void Clear ()
    {
        if ( myFireScript != null )
        {
            myFireScript.Clear();
        }
        Destroy(myGameObject);
    }

    void Store ()
    {
        if (GameManager.instance != null && GameManager.instance.allFlameableObjects != null)
        {
            GameManager.instance.allFlameableObjects.Add(this);
        }
    }
}
