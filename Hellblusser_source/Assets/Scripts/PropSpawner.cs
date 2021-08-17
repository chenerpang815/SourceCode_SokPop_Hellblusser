using UnityEngine;
using System.Collections.Generic;

public class PropSpawner : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // spawning
    public enum Prop
    {
        Pot,
        Urn,
        Chest,
        Crate,
        KeyChest,
        Barrel,
        SewerRubble,
        SewerPlant,
        DungeonRubble,
        DungeonPlant,
        Cage,
        BonePile,
        EyeBall,
        HellPlant,
        HellGeyser,
        MoonStatue,
        HellPot,
    };

    public bool preventFireOnStart;

    public List<PropData> possiblePropDatas;

    [System.Serializable]
    public struct PropData
    {
        public Prop propType;
        public int weight;
    };

    // generated
    bool generated;

    void Start ()
    {
        // base
        myTransform = transform;
        myGameObject = gameObject;

        // store
        Store();

        // generate?
        if ( SetupManager.instance.curGameState == SetupManager.GameState.Shop || SetupManager.instance.curGameState == SetupManager.GameState.Rest || SetupManager.instance.curGameState == SetupManager.GameState.Outro )
        {
            Generate();
        }

        // state
        generated = false;
    }

    public void Generate ()
    {
        //return;

        // spawn
        WeightedRandomBag<Prop> possibleProps = new WeightedRandomBag<Prop>();
        for (int i = 0; i < possiblePropDatas.Count; i++)
        {
            PropData curPropData = possiblePropDatas[i];
            possibleProps.AddEntry(curPropData.propType, curPropData.weight);
        }

        GameObject propSpawnO = null;
        Prop propSpawn = possibleProps.ChooseGeneration("propSpawnType");
        switch (propSpawn)
        {
            case Prop.Pot: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.potPrefab); break;
            case Prop.Urn: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.urnPrefab); break;
            case Prop.Chest: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.chestPrefab); break;
            case Prop.Crate: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.cratePrefab); break;
            case Prop.Barrel: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.barrelPrefab); break;
            case Prop.KeyChest: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.keyChestPrefab); break;
            case Prop.SewerRubble: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.sewerRubblePrefab); break;
            case Prop.SewerPlant: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.sewerPlantPrefab); break;
            case Prop.DungeonRubble: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.dungeonRubblePrefab); break;
            case Prop.DungeonPlant: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.dungeonPlantPrefab); break;
            case Prop.Cage: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.cagePrefab); break;
            case Prop.BonePile: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.bonePilePrefab); break;
            case Prop.EyeBall: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.eyeBallPrefab); break;
            case Prop.HellPlant: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.hellPlantPrefab); break;
            case Prop.HellGeyser: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.hellGeyserPrefab); break;
            case Prop.MoonStatue: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.moonStatuePrefab); break;
            case Prop.HellPot: propSpawnO = BasicFunctions.PickRandomObjectFromArrayGeneration(PrefabManager.instance.hellPotPrefab); break;
        }

        // ENNE DAN SPAWNE
        float spawnOffMax = .125f;
        Vector3 spawnP = myTransform.position;
        spawnP.x += (spawnOffMax * -.5f) + TommieRandom.instance.GenerationRandomValue(spawnOffMax * 2f,"spawn off x"); //TommieRandom.instance.GenerationRandomRange(-spawnOffMax, spawnOffMax, "prop spawn off x");
        spawnP.z += (spawnOffMax * -.5f) + TommieRandom.instance.GenerationRandomValue(spawnOffMax * 2f, "spawn off z");//TommieRandom.instance.GenerationRandomRange(-spawnOffMax, spawnOffMax, "prop spawn off z");
        Quaternion spawnR = Quaternion.Euler(0f, TommieRandom.instance.GenerationRandomValue(360f, "prop spawn rot off"), 0f);

        float pDst = 1f;
        Vector3 p0 = spawnP;
        p0.y += pDst;
        Vector3 p1 = spawnP;
        p1.y -= pDst;
        RaycastHit pHit;
        if ( Physics.Linecast(p0,p1,out pHit,SetupManager.instance.propAttachLayerMask) )
        {
            spawnP = pHit.point;
            spawnR = Quaternion.LookRotation(pHit.normal) * Quaternion.Euler(0f,TommieRandom.instance.GenerationRandomValue(360f,"prop hit ground spawn rot"),0f);
        }

        GameObject propO = PrefabManager.instance.SpawnPrefabAsGameObject(propSpawnO, spawnP, spawnR, 1f);
        if (preventFireOnStart)
        {
            if (propO.GetComponent<FlameableScript>() != null)
            {
                FlameableScript propFlameableScript = propO.GetComponent<FlameableScript>();
                if (propFlameableScript != null)
                {
                    propFlameableScript.preventFireOnStart = true;
                }
            }
        }

        // done
        generated = true;
    }

    public void Store ()
    {
        if ( SetupManager.instance != null && SetupManager.instance.allPropSpawners != null )
        {
            SetupManager.instance.allPropSpawners.Add(this);
        }
    }

    public void Clear ()
    {
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }

    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.blue;
        if ( possiblePropDatas != null && possiblePropDatas.Count > 0 )
        {
            if ( possiblePropDatas[0].propType == Prop.SewerPlant || possiblePropDatas[0].propType == Prop.DungeonPlant || possiblePropDatas[0].propType == Prop.HellPlant )
            {
                Color plantCol = Color.Lerp(Color.blue,Color.green,.75f);
                Gizmos.color = plantCol;
            }
        }
        Gizmos.DrawWireCube(transform.position, Vector3.one * .25f);
    }
}
