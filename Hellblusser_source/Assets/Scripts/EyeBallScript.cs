using UnityEngine;

public class EyeBallScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // eyelids
    public Transform[] eyelidTransforms;
    Quaternion[] eyelidRotOriginal; 

    // blink
    int blinkIndex, blinkWaitDurMin, blinkWaitDurMax, blinkWaitDur, blinkWaitCounter, blinkHoldDur, blinkHoldCounter;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // blink
        blinkIndex = 0;
        blinkWaitDurMin = 150;
        blinkWaitDurMax = 300;
        blinkWaitDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(blinkWaitDurMin,blinkWaitDurMax));
        blinkWaitCounter = 0;
        blinkHoldDur = 12;
        blinkHoldCounter = 0;

        // animation
        if ( eyelidTransforms != null && eyelidTransforms.Length > 0 )
        {
            eyelidRotOriginal = new Quaternion[eyelidTransforms.Length];
            for ( int i = 0; i < eyelidTransforms.Length; i ++ )
            {
                eyelidRotOriginal[i] = eyelidTransforms[i].localRotation;
            }
        }
    }

    void Update ()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            // blink
            bool close = false;
            if ( SetupManager.instance.runDataRead.curLevelIndex == SetupManager.instance.runDataRead.curFloorData.locationCount )
            {
                if (LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.curBossCore != null)
                {
                    if (LevelGeneratorManager.instance.activeLevelGenerator.curBossCore.stageIndex > 0 && LevelGeneratorManager.instance.activeLevelGenerator.curBossCore.clearedLava)
                    {
                        close = true;
                    }
                }
            }

            if (!close)
            {
                if (blinkIndex == 0)
                {
                    if (blinkWaitCounter < blinkWaitDur)
                    {
                        blinkWaitCounter++;
                    }
                    else
                    {
                        blinkIndex = 1;
                        blinkHoldCounter = 0;
                    }
                }
                else
                {
                    if (blinkHoldCounter < blinkHoldDur)
                    {
                        blinkHoldCounter++;
                    }
                    else
                    {
                        blinkIndex = 0;
                        blinkWaitCounter = 0;
                    }
                }
            }
            else
            {
                blinkIndex = 1;
            }

            // animation
            if ( eyelidTransforms != null && eyelidTransforms.Length > 0 )
            {
                float eyelidLerpie = 20f;
                for ( int i = 0; i < eyelidTransforms.Length; i ++ )
                {
                    Quaternion targetRot = eyelidRotOriginal[i];
                    if ( blinkIndex == 1 )
                    {
                        targetRot *= Quaternion.Euler(0f,0f,55f);
                    }
                    eyelidTransforms[i].localRotation = Quaternion.Lerp(eyelidTransforms[i].localRotation,targetRot,eyelidLerpie * Time.deltaTime);
                }
            }
        }
    }
}
