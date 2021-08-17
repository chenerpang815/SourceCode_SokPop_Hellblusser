using UnityEngine;

public class VisualsDatabase : MonoBehaviour
{
    // instance
    public static VisualsDatabase instance;

    // materials
    [Header("rat")]
    public Material ratLegMat;
    public Material ratTailMat;
    public AnimationCurve ratTailCurve;
    public Mesh ratFootMesh;
    public Material[] npcHitMaterials;

    void Awake ()
    {
        instance = this;
    }
}
