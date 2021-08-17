using UnityEngine;

public class FireParticles : MonoBehaviour
{
    Transform myTransform;

    public Transform[] subtractTransforms;
    Vector3[] subtractPosOriginal;
    Vector3[] subtractSclOriginal;

    void Start()
    {
        myTransform = transform;

        subtractPosOriginal = new Vector3[2];
        subtractSclOriginal = new Vector3[2];
        for (int i = 0; i < subtractTransforms.Length; i ++ )
        {
            subtractPosOriginal[i] = subtractTransforms[i].localPosition;
            subtractSclOriginal[i] = subtractTransforms[i].localScale;
        }
    }

    void Update ()
    {
        Vector3 p0 = myTransform.position;
        Vector3 p1 = Camera.main.transform.position;
        float dstToCam = Vector3.Distance(p0,p1);
        float dstMax = 4f;
        float dstConverted = BasicFunctions.ConvertRange(dstToCam,0f,dstMax,dstMax,0f);

        for ( int i = 0; i < subtractTransforms.Length; i ++ )
        {
            float subtractDir = (i == 0) ? 1f : -1f;
            Vector3 subtractPosTarget = subtractPosOriginal[i];
            subtractPosTarget.x += (dstConverted * subtractDir) * .0925f;
            subtractTransforms[i].localPosition = subtractPosTarget;
            subtractTransforms[i].localScale = Vector3.one * (1f + (dstConverted * .075f));
        }
    }
}
