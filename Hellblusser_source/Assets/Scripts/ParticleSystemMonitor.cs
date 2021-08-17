using UnityEngine;

public class ParticleSystemMonitor : MonoBehaviour
{
    // base components
    [HideInInspector] public GameObject myGameObject;
    [HideInInspector] public ParticleSystem myParticleSystem;

    // settings
    [Header("settings")]
    public bool autoClear;
    public int clearDur;
    int clearCounter;

    void Awake ()
    {
        myGameObject = gameObject;
        myParticleSystem = GetComponent<ParticleSystem>();

        clearCounter = 0;

        Store();
    }

    void Update()
    {
        if ( autoClear )
        {
            if ( SetupManager.instance != null && !SetupManager.instance.inFreeze )
            {
                if ( clearCounter < clearDur )
                {
                    clearCounter++;
                }
                else
                {
                    Clear();
                }
            }
        }
    }

    public void Store ()
    {
        if ( SetupManager.instance != null && SetupManager.instance.allParticleSystems != null )
        {
            if (!SetupManager.instance.allParticleSystems.Contains(myParticleSystem))
            {
                SetupManager.instance.allParticleSystems.Add(myParticleSystem);
            }
        }
    }

    public void Remove ()
    {
        if (SetupManager.instance != null && SetupManager.instance.allParticleSystems != null)
        {
            if (SetupManager.instance.allParticleSystems.Contains(myParticleSystem))
            {
                SetupManager.instance.allParticleSystems.Remove(myParticleSystem);
            }
        }
        if ( myParticleSystem != null )
        {
            Destroy(myParticleSystem);
        }
    }

    public void Clear ()
    {
        Remove();
        Destroy(myGameObject);
    }

    public void OnParticleSystemStopped()
    {
        Clear();
    }
}
