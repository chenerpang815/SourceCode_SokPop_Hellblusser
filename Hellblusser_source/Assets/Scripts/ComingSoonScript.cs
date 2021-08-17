using UnityEngine;
using TMPro;

public class ComingSoonScript : MonoBehaviour
{
    public TextMeshProUGUI comingSoonText;

    void Update()
    {
        if ( InputManager.instance != null )
        {
            comingSoonText.enabled = InputManager.instance.interactHold;
        }
    }
}
