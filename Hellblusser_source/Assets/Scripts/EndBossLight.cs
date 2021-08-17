using UnityEngine;

public class EndBossLight : MonoBehaviour
{
    public Light myLight;

    void Awake ()
    {
        if ( LevelGeneratorManager.instance != null )
        {
            LevelGeneratorManager.instance.endBossLight = myLight;
        }
    }
}
