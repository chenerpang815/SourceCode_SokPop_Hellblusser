using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    // base components
    [Header("base components")]
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // state
    public enum ObjectState { Default, Hold };
    public ObjectState curObjectState;

    // hold data
    public HandScript holdByScript;

    public FlameableScript myFlameableScript;

    // offset
    [Header("offset")]
    public Vector3 holdOffset;
    [HideInInspector] public Vector3 rotOffset;

    // scaling
    [Header("scaling")]
    public float scaleDefault;
    public float scaleHold;
    Vector3 scaleOriginal;

    // init
    [HideInInspector] public bool initialized;

    public void Init()
    {
        myTransform = transform;
        myGameObject = gameObject;

        // scaling
        scaleOriginal = myTransform.localScale;

        // done
        initialized = true;
    }

    void Update()
    {
        if ( !initialized )
        {
            Init();
        }
        else
        {
            // hold by hand?
            if ( curObjectState == ObjectState.Hold )
            {
                Vector3 pSet = holdByScript.myTransform.position;
                pSet += holdByScript.myTransform.right * holdOffset.x;
                pSet += holdByScript.myTransform.up * holdOffset.y;
                pSet += holdByScript.myTransform.forward * holdOffset.z;
                myTransform.position = pSet;

                Quaternion rSet = holdByScript.myTransform.rotation;
                myTransform.rotation = rSet * Quaternion.Euler(-90f,0f,0f) * Quaternion.Euler(0f,0f,-90f) * Quaternion.Euler(0f,10f * holdByScript.handDir,0f) * Quaternion.Euler(rotOffset);
            }
        }
    }

    public void SetObjectState ( ObjectState _to )
    {
        curObjectState = _to;

        Vector3 scaleSet = scaleOriginal;
        switch ( _to )
        {
            case ObjectState.Default: scaleSet = scaleOriginal * scaleDefault; break;
            case ObjectState.Hold: scaleSet = Vector3.one * scaleHold; break;
        }
        myTransform.localScale = scaleSet;
    }

    public void Grab ( HandScript _by )
    {
        SetObjectState(ObjectState.Hold);
        holdByScript = _by;
    }

    public void Release ()
    {
        SetObjectState(ObjectState.Default);
        holdByScript = null;
    }

    public void Clear ()
    {
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }
}
