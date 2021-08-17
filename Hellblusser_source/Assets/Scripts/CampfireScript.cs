using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireScript : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;
    }

    void Update ()
    {
        
    }
}
