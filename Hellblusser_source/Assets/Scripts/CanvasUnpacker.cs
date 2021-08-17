using UnityEngine;
using System.Collections.Generic;

public class CanvasUnpacker : MonoBehaviour
{
    void Awake ()
    {
        Transform myTransform = transform;
        Transform myParent = myTransform.parent;

        List<Transform> childs = new List<Transform>();
        int count = myTransform.childCount;
        for (int i = 0; i < count; i++)
        {
            childs.Add(myTransform.GetChild(i));
        }
        for ( int i = childs.Count - 1; i >= 0; i -- )
        {
            if ( childs[i].GetComponent<RectTransform>() != null )
            {
                childs[i].GetComponent<RectTransform>().SetParent(myParent);
            }
        }
        childs.Clear();
    }
}
