using System.Collections.Generic;
using UnityEngine;

namespace LevelGenerator.Scripts
{
    public class Bounds : MonoBehaviour
    {
        public IEnumerable<Collider> Colliders => GetComponentsInChildren<Collider>();

        void Awake ()
        {
            foreach ( Transform t in transform )
            {
                //t.parent = Grid.instance.platformAttachedTo.boundsContainerTransform;
                t.parent = LevelGeneratorManager.instance.activeLevelGenerator.boundsContainerTransform;
                Vector3 p = t.position;
                p.y = 0f;
                t.position = p;

                float scaleFac = .99f;
                t.localScale = (t.localScale * scaleFac);

                /*
                if (t.GetComponent<MeshRenderer>() != null)
                {
                    MeshRenderer mr = t.GetComponent<MeshRenderer>();
                    if ( mr != null )
                    {
                        mr.enabled = false;
                    }
                }
                */
            }
        }
    }
}
