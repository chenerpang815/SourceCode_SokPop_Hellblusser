using UnityEngine;

public class CopyCameraRotation : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;

    [Header("rotation offset")]
    public Vector3 rotOffset;

    [Header("lock")]
    public bool lockX;
    public bool lockY;
    public bool lockZ;

    void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        Vector3 r1 = Camera.main.transform.forward;
        if ( lockX )
        {
            r1.y = 0f;
        }
        Vector3 r2 = r1.normalized;
        Quaternion targetRot = Quaternion.LookRotation(r2) * Quaternion.Euler(rotOffset);

        myTransform.rotation = targetRot;
    }
}
