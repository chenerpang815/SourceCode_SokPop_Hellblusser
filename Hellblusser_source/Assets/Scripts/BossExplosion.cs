using UnityEngine;
using System.Collections.Generic;

public class BossExplosion : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // orbs
    List<GameObject> orbObjects;

    // settings
    public float orbRadius;

    // animation
    int orbSpawnRate, orbSpawnCounter;

    // clear
    int clearDur, clearCounter;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // animation
        orbSpawnRate = 6;
        orbSpawnCounter = 0;

        // clear
        clearDur = SetupManager.instance.bossDefeatFreeze;
        clearCounter = 0;

        // orbs
        orbObjects = new List<GameObject>();
    }

    void Update ()
    {
        if ( !SetupManager.instance.paused )
        {
            if ( clearCounter < clearDur )
            {
                clearCounter++;

                if ( orbSpawnCounter < orbSpawnRate )
                {
                    orbSpawnCounter++;
                }
                else
                {
                    Vector3 p = myTransform.position;
                    float offMax = (orbRadius * .25f);
                    float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f,1f));
                    float yDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                    float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                    float xAdd = TommieRandom.instance.RandomRange(offMax * .25f,offMax);
                    float yAdd = TommieRandom.instance.RandomRange(offMax * .25f, offMax);
                    float zAdd = TommieRandom.instance.RandomRange(offMax * .25f, offMax);
                    p.x += (xAdd * xDir);
                    p.y += (yAdd * yDir);
                    p.z += (zAdd * zDir);

                    GameObject orbO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.whiteOrbPrefab,p,Quaternion.identity,orbRadius);
                    //orbObjects.Add(orbO);

                    orbSpawnCounter = 0;

                    // audio
                    AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireMagicImpactClips),.9f,1.2f,.2f,.225f);
                }
            }
            else
            {
                AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.bossDefeatedClips),.7f,.9f,.5f,.525f);

                Clear();
            }
        }
    }

    public void Clear ()
    {
        if ( orbObjects != null && orbObjects.Count > 0 )
        {
            for ( int i = orbObjects.Count - 1; i >= 0; i -- )
            {
                Destroy(orbObjects[i]);
            }
            orbObjects.Clear();
        }
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }
}
