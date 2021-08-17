using UnityEngine;

public class LavaBallManager : MonoBehaviour
{
    // state
    int spawnBallWaitMin, spawnBallWaitMax, spawnBallWait, spawnBallCounter;
    bool foundSpawnPos;

    // layerMasks
    [Header("layerMasks")]
    public LayerMask checkLayerMask;

    void Start ()
    {
        spawnBallWaitMin = 32;
        spawnBallWaitMax = 64;
        spawnBallWait = Mathf.RoundToInt(TommieRandom.instance.RandomRange(spawnBallWaitMin,spawnBallWaitMax));
        spawnBallCounter = 0;
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze && SetupManager.instance.runDataRead.curFloorIndex == 3)
        {
            bool preventSpawn = false;

            if ( LevelGeneratorManager.instance.activeLevelGenerator.curBossCore != null )
            {
                if ( LevelGeneratorManager.instance.activeLevelGenerator.curBossCore.stageIndex > 0 )
                {
                    preventSpawn = true;
                }
            }

            if (!preventSpawn)
            {
                if (spawnBallCounter < spawnBallWait)
                {
                    foundSpawnPos = false;
                    spawnBallCounter++;
                }
                else
                {
                    Vector3 p0 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                    float r0 = 20f;
                    Vector3 d1 = TommieRandom.instance.RandomInsideSphere();
                    d1.y = 0f;
                    d1.Normalize();

                    Vector3 p1 = p0 + (d1 * TommieRandom.instance.RandomRange(r0 * .25f, r0));
                    p1.y = 0f;

                    if (!foundSpawnPos)
                    {
                        float cDst = 10f;
                        Vector3 c0 = p1;
                        c0.y += cDst;
                        Vector3 c1 = p1;
                        c1.y -= cDst;
                        if (!Physics.Linecast(c0, c1, checkLayerMask))
                        {
                            Vector3 c2 = c1;
                            c2.y = LevelGeneratorManager.instance.lavaTransform.position.y;
                            SpawnLavaBall(c2);
                            foundSpawnPos = true;
                        }
                        Debug.DrawLine(c0, c1, Color.red, 4f);
                    }

                    if (foundSpawnPos)
                    {
                        spawnBallWait = Mathf.RoundToInt(TommieRandom.instance.RandomRange(spawnBallWaitMin, spawnBallWaitMax));
                        spawnBallCounter = 0;
                    }
                }
            }
        }
    }

    void SpawnLavaBall ( Vector3 _p )
    {
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.lavaBallPrefab[0],_p,Quaternion.identity,1f);
    }
}
