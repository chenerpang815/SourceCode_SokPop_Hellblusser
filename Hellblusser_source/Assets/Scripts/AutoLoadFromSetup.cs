using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadFromSetup : MonoBehaviour
{
    void Awake ()
    {
        if ( SetupManager.instance == null )
        {
            // reset input?
            if ( InputManager.instance != null )
            {
                InputManager.instance.DisableAllInput();
            }

            // go
            SceneManager.LoadScene(0 + 1,LoadSceneMode.Single);
        }
    }
}
