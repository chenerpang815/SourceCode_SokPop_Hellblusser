using UnityEngine;

public class LevelGeneratorManager : MonoBehaviour
{
    // instance
    public static LevelGeneratorManager instance;

    // sewer references
    [Header("sewer")]
    public TommieLevelGenerator sewerSmallLevelGenerator;
    public TommieLevelGenerator sewerMediumLevelGenerator;
    public TommieLevelGenerator sewerBigLevelGenerator;
    public TommieLevelGenerator sewerBossLevelGenerator;

    // dungeon references
    [Header("dungeon")]
    public TommieLevelGenerator dungeonSmallLevelGenerator;
    public TommieLevelGenerator dungeonMediumLevelGenerator;
    public TommieLevelGenerator dungeonBigLevelGenerator;
    public TommieLevelGenerator dungeonBossLevelGenerator;

    // hell references
    [Header("hell")]
    public TommieLevelGenerator hellSmallLevelGenerator;
    public TommieLevelGenerator hellMediumLevelGenerator;
    public TommieLevelGenerator hellBigLevelGenerator;
    public TommieLevelGenerator hellBossLevelGenerator;

    // lava reference
    [Header("lava")]
    public Transform lavaTransform;
    public GameObject lavaObject;
    public MeshRenderer lavaMeshRenderer;
    public Color lavaColNormal;
    public Color lavaColDead;

    // end boss light
    [Header("light")]
    public Light endBossLight;
    public Color endBossLightColNormal;
    public Color endBossLightColDead;

    // active
    [HideInInspector] public TommieLevelGenerator activeLevelGenerator;

    void Awake ()
    {
        instance = this;

        // activate a level generator
        activeLevelGenerator = null;
        ActivateLevelGenerator();
    }

    void ActivateLevelGenerator ()
    {
        if (SetupManager.instance != null)
        {
            SetupManager.FloorType curFloorType = SetupManager.FloorType.Sewer;
            switch (SetupManager.instance.runDataRead.curFloorIndex)
            {
                case 1: curFloorType = SetupManager.FloorType.Sewer; break;
                case 2: curFloorType = SetupManager.FloorType.Dungeon; break;
                case 3: curFloorType = SetupManager.FloorType.Hell; break;
            }
            switch (curFloorType)
            {
                // SEWER
                case SetupManager.FloorType.Sewer:
                    switch (SetupManager.instance.curEncounterType)
                    {
                        case SetupManager.EncounterType.Small: activeLevelGenerator = sewerSmallLevelGenerator; break;
                        case SetupManager.EncounterType.Medium: activeLevelGenerator = sewerMediumLevelGenerator; break;
                        case SetupManager.EncounterType.Big: activeLevelGenerator = sewerBigLevelGenerator; break;
                        case SetupManager.EncounterType.Boss: activeLevelGenerator = sewerBossLevelGenerator; break;
                    }
                break;

                // DUNGEON
                case SetupManager.FloorType.Dungeon:
                    switch (SetupManager.instance.curEncounterType)
                    {
                        case SetupManager.EncounterType.Small: activeLevelGenerator = dungeonSmallLevelGenerator; break;
                        case SetupManager.EncounterType.Medium: activeLevelGenerator = dungeonMediumLevelGenerator; break;
                        case SetupManager.EncounterType.Big: activeLevelGenerator = dungeonBigLevelGenerator; break;
                        case SetupManager.EncounterType.Boss: activeLevelGenerator = dungeonBossLevelGenerator; break;
                    }
                    break;

                // HELL
                case SetupManager.FloorType.Hell:
                    switch (SetupManager.instance.curEncounterType)
                    {
                        case SetupManager.EncounterType.Small: activeLevelGenerator = hellSmallLevelGenerator; break;
                        case SetupManager.EncounterType.Medium: activeLevelGenerator = hellMediumLevelGenerator; break;
                        case SetupManager.EncounterType.Big: activeLevelGenerator = hellBigLevelGenerator; break;
                        case SetupManager.EncounterType.Boss: activeLevelGenerator = hellBossLevelGenerator; break;
                    }
                    break;
            }

            // activate lava?
            if ( lavaObject != null )
            {
                lavaObject.SetActive(curFloorType == SetupManager.FloorType.Hell);
            }

            if (activeLevelGenerator != null)
            {
                activeLevelGenerator.Init();
            }
        }
    }

    void Update ()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            if (activeLevelGenerator != null && activeLevelGenerator.curBossCore != null && activeLevelGenerator.curBossCore.clearedLava )
            {
                // lava
                if (lavaMeshRenderer != null)
                {
                    float lavaLerpie = .5f;
                    Color lavaColTarget = (activeLevelGenerator.curBossCore.stageIndex > 0) ? lavaColDead : lavaColNormal;
                    Color colSet = Color.Lerp(lavaMeshRenderer.material.GetColor("_BaseColor"), lavaColTarget, lavaLerpie * Time.deltaTime);
                    lavaMeshRenderer.material.SetColor("_BaseColor",colSet);
                }

                // end boss light
                if (endBossLight != null)
                {
                    float lightLerpie = .5f;
                    Color endBossLightColTarget = (activeLevelGenerator.curBossCore.stageIndex > 0) ? endBossLightColDead : endBossLightColNormal;
                    endBossLight.color = Color.Lerp(endBossLight.color,endBossLightColTarget,lightLerpie * Time.deltaTime);
                }
            }
        }
    }
}
