using UnityEngine;

public class SineRotation : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;

    // settings
    [Header("settings")]
    public Vector3 time;
    public Vector3 dst;

    Quaternion rotOriginal;

    void Start ()
    {
        myTransform = transform;
        if (myTransform != null)
        {
            rotOriginal = myTransform.localRotation;
        }
    }

    void LateUpdate ()
    {
        if (!SetupManager.instance.inFreeze)
        {
            if (myTransform != null)
            {
                float tt = Time.time;
                float tX = (time.x * tt);
                float tY = (time.y * tt);
                float tZ = (time.z * tt);
                float sX = Mathf.Sin(tX) * dst.x;
                float sY = Mathf.Sin(tY) * dst.y;
                float sZ = Mathf.Sin(tZ) * dst.z;
                myTransform.localRotation = rotOriginal * Quaternion.Euler(sX, sY, sZ);
            }
        }
    }
}
