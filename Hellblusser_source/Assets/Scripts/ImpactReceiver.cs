using UnityEngine;

public class ImpactReceiver : MonoBehaviour 
{
    Transform myTransform;

	Vector3 impact;
    Vector3 kickImpact;
    public CharacterController character;

    [HideInInspector] public bool initialized;

    void Awake ()
    {
        initialized = false;
    }

    void Init ()
    {
        myTransform = transform;

        // done
        initialized = true;
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze)
        {
            if (initialized)
            {
                impact = Vector3.Lerp(impact, Vector3.zero, 10f * Time.deltaTime);
                kickImpact = Vector3.Lerp(kickImpact,Vector3.zero,20f * Time.deltaTime);
                if (impact.magnitude > 0f)
                {
                    character.Move(impact * Time.deltaTime);
                }
                if (kickImpact.magnitude > 0f)
                {
                    character.Move(kickImpact * Time.deltaTime);
                }
            }
            else
            {
                if (GameManager.instance != null)
                {
                    Init();
                }
            }
        }
    }

    public void AddImpact ( Vector3 _dir, float _force )
	{
        float f = _force;
        f += SetupManager.instance.playerEquipmentStatsTotal.knockbackAdd;

        Vector3 d = Vector3.ClampMagnitude(_dir,1f);
		impact.x += (d.x * f);
        impact.y += (d.y * f);
        impact.z += (d.z * f);
    }

    public void AddKickImpact ( Vector3 _dir, float _force )
    {
        Vector3 d = Vector3.ClampMagnitude(_dir, 1f);
        kickImpact.x += (d.x * _force);
        kickImpact.y += (d.y * _force);
        kickImpact.z += (d.z * _force);
    }
}
