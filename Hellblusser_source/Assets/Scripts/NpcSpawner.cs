using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    public List<Npc.Type> possibleTypes;
    public List<Npc.Type> npcsTypesExclude;
    public bool isBossSpawner;
    public bool overrideSpawnTypes;
    public MovementTransformContainer movementTransformContainer;
    public MinionSpawnTransformContainer minionSpawnTransformContainer;
    public CannonScriptContainer cannonScriptContainer;
    public NpcSpawner dungeonBossNpcSpawner;
    public EnterNextStageMovePoint enterNextStageMovePoint;

    public List<NpcCore> myNpcCores;
    [HideInInspector] public List<bool> myNpcsDefeated;

    // state
    bool generated;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // store
        Store();

        // state 
        generated = false;
    }

    public void Generate ()
    {
        if (!SetupManager.instance.noNpcs)
        {
            SetupManager.instance.UpdateRunDataRead();

            // intialize
            myNpcCores = new List<NpcCore>();
            myNpcsDefeated = new List<bool>();

            // choose an encounter archetype
            if (!overrideSpawnTypes)
            {
                int encountersHadCheck = (SetupManager.instance.runDataRead.encountersHad + 1);

                SetupManager.EncounterArchetype encounterArchetypeUse = SetupManager.EncounterArchetype.SewerGeneric;
                switch (SetupManager.instance.runDataRead.curFloorIndex)
                {
                    // SEWER
                    case 1:
                        switch (encountersHadCheck)
                        {
                            case 1: encounterArchetypeUse = SetupManager.EncounterArchetype.SewerGeneric; break;
                            case 2: encounterArchetypeUse = SetupManager.EncounterArchetype.Slug; break;
                            case 3: encounterArchetypeUse = SetupManager.EncounterArchetype.Goblin; break;
                        }
                        if (encountersHadCheck > 3)
                        {
                            WeightedRandomBag<SetupManager.EncounterArchetype> temp = new WeightedRandomBag<SetupManager.EncounterArchetype>();
                            temp.AddEntry(SetupManager.EncounterArchetype.SewerGeneric,10);
                            temp.AddEntry(SetupManager.EncounterArchetype.Slug, 10);
                            temp.AddEntry(SetupManager.EncounterArchetype.Goblin, 15);
                            encounterArchetypeUse = temp.ChooseGeneration("encounterArchetype");
                        }
                        break;

                    // DUNGEON
                    case 2:
                        switch (encountersHadCheck)
                        {
                            case 1: encounterArchetypeUse = SetupManager.EncounterArchetype.DungeonGeneric; break;
                            case 2: encounterArchetypeUse = SetupManager.EncounterArchetype.DungeonSpider; break;
                            case 3: encounterArchetypeUse = SetupManager.EncounterArchetype.DungeonOrc; break;
                        }
                        if (encountersHadCheck > 3)
                        {
                            WeightedRandomBag<SetupManager.EncounterArchetype> temp = new WeightedRandomBag<SetupManager.EncounterArchetype>();
                            temp.AddEntry(SetupManager.EncounterArchetype.DungeonGeneric, 10);
                            temp.AddEntry(SetupManager.EncounterArchetype.DungeonSpider, 10);
                            temp.AddEntry(SetupManager.EncounterArchetype.DungeonOrc, 15);
                            encounterArchetypeUse = temp.ChooseGeneration("encounterArchetype");
                        }
                        break;

                    // HELL
                    case 3:
                        switch (encountersHadCheck)
                        {
                            case 1: encounterArchetypeUse = SetupManager.EncounterArchetype.HellGeneric; break;
                            case 2: encounterArchetypeUse = SetupManager.EncounterArchetype.HellSkeleton; break;
                            case 3: encounterArchetypeUse = SetupManager.EncounterArchetype.HellFire; break;
                        }
                        if (encountersHadCheck > 3)
                        {
                            WeightedRandomBag<SetupManager.EncounterArchetype> temp = new WeightedRandomBag<SetupManager.EncounterArchetype>();
                            temp.AddEntry(SetupManager.EncounterArchetype.HellGeneric, 15);
                            temp.AddEntry(SetupManager.EncounterArchetype.HellSkeleton, 10);
                            temp.AddEntry(SetupManager.EncounterArchetype.HellFire, 10);
                            encounterArchetypeUse = temp.ChooseGeneration("encounterArchetype");
                        }
                        break;
                }

                // archetype?
                //Debug.Log("current archetype: " + encounterArchetypeUse + " || " + Time.time.ToString());

                // get possible npc types
                SetupManager.EncounterArchetypeData encouterArchetypeDataUse = SetupManager.instance.GetEncounterArchetypeData(encounterArchetypeUse);
                WeightedRandomBag<Npc.Type> genericSpawnTypesUse = encouterArchetypeDataUse.npcTypes;
                WeightedRandomBag<Npc.Type> bossSpawnTypesUse = encouterArchetypeDataUse.bossNpcTypes;

                // last level of biome?
                if ((SetupManager.instance.runDataRead.curLevelIndex + 1) == (SetupManager.instance.runDataRead.curFloorData.locationCount))
                {
                    bossSpawnTypesUse = new WeightedRandomBag<Npc.Type>();
                    switch (SetupManager.instance.runDataRead.curFloorIndex)
                    {
                        case 1: bossSpawnTypesUse.AddEntry(Npc.Type.RatKing, 10f); break;
                        case 2: bossSpawnTypesUse.AddEntry(Npc.Type.MagicSkull, 10f); break;
                        case 3: bossSpawnTypesUse.AddEntry(Npc.Type.HellLord, 10f); break;
                    }
                }

                // add some generic enemies to the pool
                WeightedRandomBag<Npc.Type> smallExtraTypes = new WeightedRandomBag<Npc.Type>();
                switch (SetupManager.instance.runDataRead.curFloorIndex)
                {
                    // SEWER
                    case 1:

                        // SEWER GENERIC ARCHETYPE
                        switch ( encounterArchetypeUse )
                        {
                            case SetupManager.EncounterArchetype.SewerGeneric:
                                genericSpawnTypesUse.AddEntry(Npc.Type.Bat, 5);
                                if (SetupManager.instance.runDataRead.encountersHad > 1)
                                {
                                    genericSpawnTypesUse.AddEntry(Npc.Type.Ghost, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.SmallRockGolem, 5);
                                }
                                if (SetupManager.instance.runDataRead.encountersHad > 3)
                                {
                                    genericSpawnTypesUse.AddEntry(Npc.Type.Goblin, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.GoblinMage, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.GoblinRanger, 5);
                                }

                                smallExtraTypes.AddEntry(Npc.Type.Bat, 5);
                                smallExtraTypes.AddEntry(Npc.Type.Slug, 10);
                                smallExtraTypes.AddEntry(Npc.Type.SmallRat, 10);
                                break;
                        }

                        // SEWER SLUG ARCHETYPE
                        switch (encounterArchetypeUse)
                        {
                            case SetupManager.EncounterArchetype.SewerGeneric:
                                genericSpawnTypesUse.AddEntry(Npc.Type.Slug, 5);
                                if (SetupManager.instance.runDataRead.encountersHad > 1)
                                {
                                    genericSpawnTypesUse.AddEntry(Npc.Type.Ghost, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.SmallRockGolem, 5);
                                }

                                smallExtraTypes.AddEntry(Npc.Type.Slug, 10);
                                break;
                        }

                        // SEWER SLUG ARCHETYPE
                        switch (encounterArchetypeUse)
                        {
                            case SetupManager.EncounterArchetype.Goblin:
                                genericSpawnTypesUse.AddEntry(Npc.Type.Bat, 5);
                                genericSpawnTypesUse.AddEntry(Npc.Type.Rat, 5);
                                if (SetupManager.instance.runDataRead.encountersHad > 1)
                                {
                                    genericSpawnTypesUse.AddEntry(Npc.Type.Ghost, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.SmallRockGolem, 5);
                                }

                                smallExtraTypes.AddEntry(Npc.Type.SmallRat, 10);
                                break;
                        }

                        break;

                    // DUNGEON
                    case 2:

                        switch ( encounterArchetypeUse )
                        {
                            // DUNGEON GENERIC
                            case SetupManager.EncounterArchetype.DungeonGeneric:
                                genericSpawnTypesUse.AddEntry(Npc.Type.Bat, 5);
                                if (SetupManager.instance.runDataRead.encountersHad > 1)
                                {
                                    genericSpawnTypesUse.AddEntry(Npc.Type.Ghost, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.SmallRockGolem, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.BigSlug, 5);
                                }

                                smallExtraTypes.AddEntry(Npc.Type.Bat, 10);
                                smallExtraTypes.AddEntry(Npc.Type.Slug, 10);
                                smallExtraTypes.AddEntry(Npc.Type.SmallSpider, 10);
                                break;

                            // DUNGEON ORC
                            case SetupManager.EncounterArchetype.DungeonOrc:
                                genericSpawnTypesUse.AddEntry(Npc.Type.Bat, 5);
                                if (SetupManager.instance.runDataRead.encountersHad > 1)
                                {
                                    genericSpawnTypesUse.AddEntry(Npc.Type.FireMage, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.Ghost, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.SmallRockGolem, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.BigSlug, 5);
                                }
                                if (SetupManager.instance.runDataRead.encountersHad > 3)
                                {
                                    genericSpawnTypesUse.AddEntry(Npc.Type.OrcWarrior, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.OrcMage, 5);
                                    smallExtraTypes.AddEntry(Npc.Type.OrcWarrior, 5);
                                    smallExtraTypes.AddEntry(Npc.Type.OrcMage, 5);
                                }

                                smallExtraTypes.AddEntry(Npc.Type.Rat, 10);
                                smallExtraTypes.AddEntry(Npc.Type.Slug, 10);
                                smallExtraTypes.AddEntry(Npc.Type.SmallRat, 10);
                                smallExtraTypes.AddEntry(Npc.Type.SmallSpider, 10);
                                break;

                            // DUNGEON SPIDER
                            case SetupManager.EncounterArchetype.DungeonSpider:
                                if (SetupManager.instance.runDataRead.encountersHad > 1)
                                {
                                    genericSpawnTypesUse.AddEntry(Npc.Type.Ghost, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.SmallRockGolem, 5);
                                    genericSpawnTypesUse.AddEntry(Npc.Type.BigSlug, 5);
                                }

                                smallExtraTypes.AddEntry(Npc.Type.SmallSpider, 10);
                                smallExtraTypes.AddEntry(Npc.Type.MediumSpider, 10);
                                break;
                        }

                        break;

                    // HELL
                    case 3:
                        genericSpawnTypesUse.AddEntry(Npc.Type.FireBat, 5);
                        if (SetupManager.instance.runDataRead.encountersHad > 1)
                        {
                            genericSpawnTypesUse.AddEntry(Npc.Type.HellPopje, 5);
                            genericSpawnTypesUse.AddEntry(Npc.Type.HellFlyingFace, 5);
                        }

                        smallExtraTypes.AddEntry(Npc.Type.FireBat, 10);
                        smallExtraTypes.AddEntry(Npc.Type.SmallFireRat, 10);
                        smallExtraTypes.AddEntry(Npc.Type.MediumSpider, 10);
                        break;
                }

                // spawn enemies
                Npc.Type spawnTypeChoose = Npc.Type.Rat;
                bool choseType = false;
                spawnTypeChoose = (isBossSpawner) ? bossSpawnTypesUse.ChooseGeneration("npcBossSpawnType") : genericSpawnTypesUse.ChooseGeneration("npcGenericSpawnType");
                if (npcsTypesExclude == null || (npcsTypesExclude != null && npcsTypesExclude.Count <= 0) || (npcsTypesExclude != null && npcsTypesExclude.Count > 0 && !npcsTypesExclude.Contains(spawnTypeChoose)))
                {
                    choseType = true;
                }
                //while (!choseType)
                //{
                //    spawnTypeChoose = (isBossSpawner) ? bossSpawnTypesUse.ChooseGeneration("npcBossSpawnType") : genericSpawnTypesUse.ChooseGeneration("npcGenericSpawnType");
                //    if (npcsTypesExclude == null || (npcsTypesExclude != null && npcsTypesExclude.Count <= 0) || (npcsTypesExclude != null && npcsTypesExclude.Count > 0 && !npcsTypesExclude.Contains(spawnTypeChoose)))
                //    {
                //        choseType = true;
                //    }
                //}

                //// override npc spawn type?? hack
                //bool overrideSpawnType = true;
                //if (overrideSpawnType)
                //{
                //    spawnTypeChoose = Npc.Type.Slug; //Npc.Type.RatKing;
                //    choseType = true;
                //}

                if (choseType)
                {
                    int spawnCount = 1;
                    if ( spawnTypeChoose == Npc.Type.Bat || spawnTypeChoose == Npc.Type.Slug || spawnTypeChoose == Npc.Type.SmallSpider || spawnTypeChoose == Npc.Type.SmallRat )
                    {
                        WeightedRandomBag<int> possibleSpawnCounts = new WeightedRandomBag<int>();
                        //possibleSpawnCounts.AddEntry(1, 10f);
                        possibleSpawnCounts.AddEntry(2, 10f);
                        possibleSpawnCounts.AddEntry(3, 5f);
                        spawnCount = possibleSpawnCounts.ChooseGeneration("small enemy spawn count define");
                    }

                    /*
                    switch (spawnTypeChoose)
                    {
                        case Npc.Type.Bat: spawnCount = Mathf.FloorToInt(TommieRandom.instance.GenerationRandomRange(1f, 2f, "small enemy for bat")); break;
                        case Npc.Type.Slug: spawnCount = Mathf.FloorToInt(TommieRandom.instance.GenerationRandomRange(2f, 3f, "small enemy for slug")); break;
                        case Npc.Type.SmallRat: spawnCount = Mathf.FloorToInt(TommieRandom.instance.GenerationRandomRange(2f, 3f, "small enemy for small rat")); break;
                        case Npc.Type.SmallSpider: spawnCount = Mathf.FloorToInt(TommieRandom.instance.GenerationRandomRange(2f, 3f, "small enemy for small spider")); break;
                    }
                    */

                    if ( SetupManager.instance.curEncounterType == SetupManager.EncounterType.Small )
                    {
                        spawnCount--;
                        if ( spawnCount < 1 )
                        {
                            spawnCount = 1;
                        }
                    }

                    //spawnCount = 2;

                    // ENNE DAN SPAWNE
                    List<Npc.Type> typesThatDontSpawnSmall = new List<Npc.Type>();
                    typesThatDontSpawnSmall.Add(Npc.Type.SmallRat);
                    typesThatDontSpawnSmall.Add(Npc.Type.SmallSpider);
                    typesThatDontSpawnSmall.Add(Npc.Type.Slug);

                    for (int i = 0; i < spawnCount; i++)
                    {
                        // spawn me!
                        GameManager.instance.SpawnNpc(spawnTypeChoose, myTransform.position, this, isBossSpawner, (i > 0), (i > 0), movementTransformContainer, minionSpawnTransformContainer, cannonScriptContainer, null);

                        // also spawn some extra friends around me :)
                        if (SetupManager.instance.curEncounterType != SetupManager.EncounterType.Boss)
                        {
                            if (!typesThatDontSpawnSmall.Contains(spawnTypeChoose))
                            {
                                GameManager.instance.SpawnNpc(smallExtraTypes.ChooseGeneration("smallExtraType"), myTransform.position, this, false, true, false, null, null, null, null);
                            }
                        }
                    }
                }
            }
            else
            {
                int rIndex = Mathf.RoundToInt(TommieRandom.instance.GenerationRandomRange(0f, possibleTypes.Count - 1, "override npc spawn type"));
                Npc.Type typeChosen = possibleTypes[rIndex];
                GameManager.instance.SpawnNpc(typeChosen, myTransform.position, this, false, true, false, null, null, null, null);
            }
        }

        // done
        generated = true;
    }

    public void Store()
    {
        if (SetupManager.instance != null && SetupManager.instance.allNpcSpawners != null)
        {
            SetupManager.instance.allNpcSpawners.Add(this);
        }
    }

    public bool CheckIfNpcsDefeated ()
    {
        for ( int i = 0; i < myNpcsDefeated.Count; i ++ )
        {
            if (!myNpcsDefeated[i])
            {
                return false;
            }
        }
        return true;
    }

    public void Clear ()
    {
        Destroy(myGameObject);
    }

    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position,Vector3.one * .25f);
    }
}
