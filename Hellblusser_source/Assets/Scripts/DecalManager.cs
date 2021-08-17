using UnityEngine;

public class DecalManager : MonoBehaviour
{
    public static DecalManager instance;

    static readonly int decalPositionsId = Shader.PropertyToID("_DecalPositions");
    static readonly int decalPositionLengthId = Shader.PropertyToID("_DecalPositionLength");
    Vector4[] positions = new Vector4[1000];
    int decalPositionLength;

    void Awake ()
    {
        instance = this;
    }

    void Start()
    {
        decalPositionLength = 0;
    }

    void Update ()
    {
        Shader.SetGlobalVectorArray(decalPositionsId,positions);
        Shader.SetGlobalInt(decalPositionLengthId,decalPositionLength);
    }

    public void AddDecal ( Vector3 _p, float _r )
    {
        if (decalPositionLength >= (1000 - 1) )
        {
            positions = new Vector4[1000];
            decalPositionLength = 0;
        }

        positions[decalPositionLength] = new Vector4(_p.x,_p.y,_p.z,_r);

        decalPositionLength++;
    }

    public void ClearAllDecals ()
    {
        decalPositionLength = 0;
    }
}
