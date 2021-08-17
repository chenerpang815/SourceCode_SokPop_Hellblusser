using UnityEngine;
using System.Collections.Generic;

public class FlyingEyeBallSpawner : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // settings
    public float radius;
    public Vector2 regionSize;
    public int rejectionSamples;
    List<Vector2> points;

    void Start ()
    {
        if (SetupManager.instance.runDataRead.curFloorIndex == 3 || SetupManager.instance.curGameState == SetupManager.GameState.Outro)
        {
            points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);

            Vector3 pOff = new Vector3(-regionSize.x * .5f,0f,-regionSize.y * .25f);

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 pGet = points[i];

                float y = TommieRandom.instance.RandomRange(10f, 20f);
                if ( SetupManager.instance.curGameState == SetupManager.GameState.Outro )
                {
                    y = TommieRandom.instance.RandomRange(8f,24f);
                }

                Vector3 p = new Vector3(pGet.x, y, pGet.y) + pOff;
                PrefabManager.instance.SpawnPrefab(BasicFunctions.PickRandomObjectFromArray(PrefabManager.instance.flyingEyeBallPrefab), p, Quaternion.identity, TommieRandom.instance.RandomRange(.75f, 1.25f));
            }

            // log
            //Debug.Log("boem ga maar || " + Time.time.ToString());
        }
        else
        {
            Clear();
        }
    }

    public void Clear ()
    {
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }
}
