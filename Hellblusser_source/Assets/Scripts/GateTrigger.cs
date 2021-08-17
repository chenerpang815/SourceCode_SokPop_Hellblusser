using System.Collections.Generic;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // settings
    [Header("settings")]
    public float radius;

    // state
    [HideInInspector] public bool hasBeenTriggered;

    // gates
    [Header("gates")]
    public List<GateScript> gateScripts;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze && !SetupManager.instance.noNpcs)
        {
            bool playerOnWrongSide = CheckIfPlayerOnWrongSide();
            bool npcOnWrongSide = CheckIfPlayerOnWrongSide();
            bool allNpcsDefeateed = CheckIfAllNpcsDefeated();

            if (!playerOnWrongSide && !npcOnWrongSide && !allNpcsDefeateed)
            {
                Vector3 p0 = myTransform.position;
                Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                p1.y = p0.y;
                float d0 = Vector3.Distance(p0, p1);
                if (d0 <= radius)
                {
                    CloseGates();
                }
            }
            else
            {
                OpenGates();
            }
        }
    }

    public void Trigger ()
    {
        OpenGates();
        hasBeenTriggered = true;
    }

    public bool CheckIfPlayerOnWrongSide ()
    {
        for (int i = 0; i < gateScripts.Count; i++)
        {
            Vector3 d0 = gateScripts[i].myTransform.right;

            Vector3 p0 = gateScripts[i].myTransform.position;
            Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
            p1.y = p0.y;
            Vector3 d1 = (p1 - p0).normalized;

            float dot0 = Vector3.Dot(d0,d1);
            float dotThreshold0 = 0f;
            if ( dot0 < dotThreshold0 )
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfNpcOnWrongSide()
    {
        for (int i = 0; i < gateScripts.Count; i++)
        {
            Vector3 d0 = gateScripts[i].myTransform.right;
            Vector3 p0 = gateScripts[i].myTransform.position;
            for (int ii = 0; ii < gateScripts[i].myNpcSpawners.Count; ii++)
            {
                for (int iii = 0; iii < gateScripts[i].myNpcSpawners[ii].myNpcCores.Count; iii++)
                {
                    Vector3 p1 = gateScripts[i].myNpcSpawners[ii].myNpcCores[iii].myTransform.position;
                    p1.y = p0.y;
                    Vector3 d1 = (p1 - p0).normalized;

                    float dot0 = Vector3.Dot(d0, d1);
                    float dotThreshold0 = 0f;
                    if (dot0 < dotThreshold0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool CheckIfAllNpcsDefeated ()
    {
        for (int i = 0; i < gateScripts.Count; i++)
        {
            return gateScripts[i].allNpcsDefeated;
        }
        return false;
    }

    public void OpenGates ()
    {
        if (gateScripts != null && gateScripts.Count > 0)
        {
            for (int i = 0; i < gateScripts.Count; i++)
            {
                gateScripts[i].SetState(GateScript.State.Open);
            }
        }
    }

    public void CloseGates ()
    {
        if (gateScripts != null && gateScripts.Count > 0)
        {
            for (int i = 0; i < gateScripts.Count; i++)
            {
                gateScripts[i].SetState(GateScript.State.Closed);
            }
        }

        // set all npcs to alerted?
        for (int i = 0; i < gateScripts.Count; i++)
        {
            for (int ii = 0; ii < gateScripts[i].myNpcSpawners.Count; ii++)
            {
                for (int iii = 0; iii < gateScripts[i].myNpcSpawners[ii].myNpcCores.Count; iii++)
                {
                    //Debug.Log("uhhh hallo ik ben een npc ik verdien dit || " + Time.time.ToString());

                    NpcCore npcCoreCheck = gateScripts[i].myNpcSpawners[ii].myNpcCores[iii];
                    if ( npcCoreCheck != null && !npcCoreCheck.inCombat && npcCoreCheck.curState != NpcCore.State.Sleeping && npcCoreCheck.curState != NpcCore.State.WakeUp )
                    {
                        npcCoreCheck.InitSetAlerted();
                    }
                }
            }
        }
    }

    public void Clear ()
    {
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,radius);
    }
}
