using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TommieLevelGenerator : MonoBehaviour
{
    // settings
    [Header("settings")]
    public TommieLevelSection startSection;

    // boss
    [HideInInspector] public NpcCore curBossCore;

    // dead ends
    [Header("dead ends")]
    public TommieLevelSectionEntry[] deadEnds;
    [HideInInspector] public WeightedRandomBag<TommieLevelSection> allDeadEnds;

    // bounds container
    [Header("boundsContainer")]
    public Transform boundsContainerTransform;
    public GameObject boundsContainerGameObject;
    List<GameObject> boundsColliding;

    // rules
    [Header("rules")]
    public TommieLevelRules[] rules;

    // rules
    [Header("must haves")]
    public TommieLevelMustHaves[] mustHaves;

    // initWait
    bool wantsToInit;
    bool triedInit;
    int initWaitDur, initWaitCounter;

    // dictionary
    public enum SectionType
    {
        Spawn,
        Finish,
        Corridor,
        Room,
        DeadEnd,
        KeyCorridor,
        KeyRoom,
        MiniBossCorridor,
        MiniBossRoom,
        BossCorridor,
        BossRoom,
        KeyRoomEnd,
        BigRoomToBoss,
        BigRoomToKey,
        FountainRoom,
        FountainRoomEnd,
    };
    [HideInInspector] public Dictionary<SectionType, int> sectionCount;

    // npcs
    [HideInInspector] public List<NpcCore> activeNpcs;

    // cannons
    [HideInInspector] public List<CannonScript> allCannonScripts;

    // section objects
    [HideInInspector] public List<GameObject> allSectionObjects;

    // state
    [HideInInspector] public bool generatedLevel;
    [HideInInspector] public int generatedSectionDur, generatedSectionCounter;

    [System.Serializable]
    public struct TommieLevelSectionEntry
    {
        public TommieLevelSection section;
        public float weight;
    };

    [System.Serializable]
    public struct TommieLevelRules
    {
        public SectionType sectionType;
        public int maxCount;
    }

    [System.Serializable]
    public struct TommieLevelMustHaves
    {
        public SectionType sectionType;
        public int minCount;
    }

    public void Init ()
    {
        triedInit = false;
        wantsToInit = true;
        initWaitDur = 10;
        initWaitCounter = 0;

        TryInit();

        /*
        // state
        generatedLevel = false;
        allSectionObjects = new List<GameObject>();
        boundsColliding = new List<GameObject>();

        // initialize lists
        activeNpcs = new List<NpcCore>();
        allCannonScripts = new List<CannonScript>();

        // dictionary
        sectionCount = new Dictionary<SectionType, int>();

        // dead ends
        allDeadEnds = new WeightedRandomBag<TommieLevelSection>();
        for ( int i = 0; i < deadEnds.Length; i ++ )
        {
            TommieLevelSectionEntry curDeadEnd = deadEnds[i];
            allDeadEnds.AddEntry(curDeadEnd.section,curDeadEnd.weight);
        }

        SetupManager.instance.UpdateRunDataRead();

        // get seed
        switch (SetupManager.instance.curRunType)
        {
            case SetupManager.RunType.Normal: TommieRandom.instance.SetGenerationSeedHard(SetupManager.instance.curProgressData.normalRunData.runSeed + (SetupManager.instance.curProgressData.normalRunData.curFloorIndex * 20) + ((SetupManager.instance.curProgressData.normalRunData.curLevelIndex + 1) * 20)); break;
            case SetupManager.RunType.Endless: TommieRandom.instance.SetGenerationSeedHard(SetupManager.instance.curProgressData.endlessRunData.runSeed + ((SetupManager.instance.curProgressData.endlessRunData.curLoopIndex + 1) * 20) + (SetupManager.instance.curProgressData.endlessRunData.curFloorIndex * 20) + ((SetupManager.instance.curProgressData.endlessRunData.curLevelIndex + 1) * 20)); break;
        }

        // start generating
        InitStartGenerate();

        // log
        Debug.Log("boem lekker init! || " + Time.time.ToString());
        */
    }

    void InitStartGenerate ()
    {
        // log
        //Debug.Log("init generation || start index: " + TommieRandom.instance.__generation_random_index.ToString() + " || levelIndex: " + SetupManager.instance.runDataRead.curLevelIndex.ToString() + " || encountersHad: " + SetupManager.instance.runDataRead.encountersHad.ToString() + " || " + Time.time.ToString());

        // clear spawners
        SetupManager.instance.ClearAllPropSpawners();
        SetupManager.instance.ClearAllNpcSpawners();
        SetupManager.instance.ClearAllFountainAreas();

        // clear things
        ClearSections();
        ClearBounds();
        ClearDictionary();

        // goooo
        generatedLevel = false;
        generatedSectionDur = 1;
        generatedSectionCounter = 0;
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel ()
    {
        generatedLevel = false;

        // spawn start section
        if (startSection != null)
        {
            Instantiate(startSection.gameObject, null, true);
            generatedSectionCounter = 0;
        }

        //Debug.Log("lekker genereren || " + Time.time.ToString());

        while (generatedSectionCounter < generatedSectionDur)
        {
            yield return null;
        }

        bool boundsColliding = CheckIfBoundsColliding();
        bool createdMustHaves = CheckIfCreatedMustHaves();

        //Debug.Log("waren er bounds die colliden?? " + boundsColliding + " || " + Time.time.ToString());

        if ( !boundsColliding && createdMustHaves )
        {
            generatedLevel = true;

            // spawners
            if ( SetupManager.instance.allPropSpawners != null )
            {
                for ( int i = 0; i < SetupManager.instance.allPropSpawners.Count; i ++ )
                {
                    SetupManager.instance.allPropSpawners[i].Generate();
                }
            }
            if (SetupManager.instance.allNpcSpawners != null)
            {
                for (int i = 0; i < SetupManager.instance.allNpcSpawners.Count; i++)
                {
                    SetupManager.instance.allNpcSpawners[i].Generate();
                }
            }

            // create grid
            Invoke("CreateGrid", .5f);

            // get max collectibles
            Invoke("GetMaxCollectibles", 1f);
        }
        else
        {
            //Debug.Log("bounds die colliden! opnieuw || " + Time.time.ToString());

            StopCoroutine(GenerateLevel());
            InitStartGenerate();
        }
    }

    bool CheckIfCreatedMustHaves ()
    {
        if (mustHaves != null && mustHaves.Length > 0)
        {
            for (int i = 0; i < mustHaves.Length; i++)
            {
                TommieLevelMustHaves mustHaveCheck = mustHaves[i];
                int countGet;
                sectionCount.TryGetValue(mustHaveCheck.sectionType, out countGet);
                if (countGet < mustHaveCheck.minCount )
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool CheckIfBoundsColliding ()
    {
        //Debug.Log("halloooo bounds checken!! || " + Time.time.ToString());

        Collider[] allBounds = boundsContainerTransform.GetComponentsInChildren<Collider>();
        for ( int i = 0; i < allBounds.Length; i ++ )
        {
            for (int ii = 0; ii < allBounds.Length; ii++)
            {
                if ( ii != i )
                {
                    bool intersecting = allBounds[i].bounds.Intersects(allBounds[ii].bounds);
                    if (intersecting)
                    {
                        //boundsColliding.Add(allBounds[i].gameObject);
                        //boundsColliding.Add(allBounds[ii].gameObject);
                        //Debug.Log(i.ToString() + " & " + ii.ToString() + " intersecting!! || " + Time.time.ToString());

                        return intersecting;
                    }
                }
            }
        }
        //Debug.Log("bounds waren niet aan het intersecting helemaal toppie || " + Time.time.ToString());
        return false;
    }

    void Update ()
    {
        if (wantsToInit && !triedInit)
        {
            if (initWaitCounter < initWaitDur)
            {
                initWaitCounter++;
            }
            else
            {
                TryInit();
            }
        }

        if ( generatedSectionCounter < generatedSectionDur )
        {
            generatedSectionCounter++;
        }
    }

    void TryInit ()
    {
        // state
        generatedLevel = false;
        allSectionObjects = new List<GameObject>();
        boundsColliding = new List<GameObject>();

        // initialize lists
        activeNpcs = new List<NpcCore>();
        allCannonScripts = new List<CannonScript>();

        // dictionary
        sectionCount = new Dictionary<SectionType, int>();

        // dead ends
        allDeadEnds = new WeightedRandomBag<TommieLevelSection>();
        for (int i = 0; i < deadEnds.Length; i++)
        {
            TommieLevelSectionEntry curDeadEnd = deadEnds[i];
            allDeadEnds.AddEntry(curDeadEnd.section, curDeadEnd.weight);
        }

        SetupManager.instance.UpdateRunDataRead();

        // get seed
        SetupManager.instance.SetRunSeedLevel();

        // start generating
        InitStartGenerate();

        // done
        triedInit = true;

        // log
        //Debug.Log("boem lekker init! || " + Time.time.ToString());
    }

    void CreateGrid ()
    {
        Grid.instance.Init();
    }

    void GetMaxCollectibles ()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.CalculateMaxCollectiblesInLevel();
        }
    }

    public void AddSection ( SectionType _type )
    {
        if ( sectionCount.ContainsKey(_type) )
        {
            int countGet;
            sectionCount.TryGetValue(_type, out countGet);
            sectionCount[_type] = (countGet + 1);
        }
        else
        {
            sectionCount.Add(_type, 1);
        }
    }

    void ClearSections ()
    {
        if ( allSectionObjects != null && allSectionObjects.Count > 0 )
        {
            for ( int i = allSectionObjects.Count - 1; i >= 0; i -- )
            {
                Destroy(allSectionObjects[i]);
            }
            allSectionObjects.Clear();
        }
    }

    void ClearBounds ()
    {
        List<GameObject> allBoundObjects = new List<GameObject>();
        foreach ( Transform t in boundsContainerTransform )
        {
            if ( t != null )
            {
                allBoundObjects.Add(t.gameObject);
            }
        }
        if (allBoundObjects != null && allBoundObjects.Count > 0)
        {
            for (int i = allBoundObjects.Count - 1; i >= 0; i--)
            {
                if (!boundsColliding.Contains(allBoundObjects[i]))
                {
                    Destroy(allBoundObjects[i]);
                }
            }
        }
    }

    void ClearDictionary ()
    {
        if (sectionCount != null)
        {
            sectionCount.Clear();
        }
        sectionCount = new Dictionary<SectionType, int>();
    }
}
