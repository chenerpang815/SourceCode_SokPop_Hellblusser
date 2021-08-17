using UnityEngine;

public class SinePosition : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;

    // settings
    [Header("settings")]
    public Vector3 time;
    public Vector3 dst;
    public bool randomTimeOffset;
    public bool randomX;
    public bool randomY;
    public bool randomZ;

    public bool isMoonOutro;

    float randomFacX, randomFacY, randomFacZ;

    float timeOffset;
    Vector3 randomFac;

    Vector3 posOriginal;

    void Start ()
    {
        myTransform = transform;
        posOriginal = myTransform.localPosition;

        if ( randomTimeOffset )
        {
            timeOffset = TommieRandom.instance.RandomRange(-10f,10f);
        }

        randomFacX = ( randomX ) ? TommieRandom.instance.RandomRange(-1f,1f) : 1f;
        randomFacY = (randomY) ? TommieRandom.instance.RandomRange(-1f, 1f) : 1f;
        randomFacZ = (randomZ) ? TommieRandom.instance.RandomRange(-1f, 1f) : 1f;
    }

    void LateUpdate ()
    {
        if (!SetupManager.instance.inFreeze)
        {
            if (myTransform != null)
            {
                bool moonTurnAway = false;
                if (isMoonOutro && OutroManager.instance != null && OutroManager.instance.shotIndex >= 4)
                {
                    moonTurnAway = true;
                }
                if (!moonTurnAway)
                {
                    float tt = Time.time + timeOffset;
                    float tX = (time.x * tt);
                    float tY = (time.y * tt);
                    float tZ = (time.z * tt);
                    float sX = Mathf.Sin(tX) * (dst.x * randomFacX);
                    float sY = Mathf.Sin(tY) * (dst.y * randomFacY);
                    float sZ = Mathf.Sin(tZ) * (dst.z * randomFacZ);
                    myTransform.localPosition = posOriginal + new Vector3(sX, sY, sZ);
                }
                else
                {
                    Vector3 p0 = myTransform.position;
                    Vector3 p1 = Camera.main.transform.position;
                    Vector3 p2 = (p1 - p0).normalized;

                    float moveAwaySpd = (7.5f * Time.deltaTime);
                    myTransform.position += (p2 * -moveAwaySpd);
                }
            }
        }
    }
}
