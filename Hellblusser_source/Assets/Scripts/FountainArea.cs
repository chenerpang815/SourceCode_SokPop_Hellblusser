using UnityEngine;

public class FountainArea : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // settings
    public float radius;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // store
        Store();
    }

    public void Store ()
    {
        if ( SetupManager.instance != null && SetupManager.instance.allFountainAreas != null )
        {
            SetupManager.instance.allFountainAreas.Add(this);
        }
    }

    public void Clear ()
    {
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }

    void OnDrawGizmos ()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position,radius);
    }
}
