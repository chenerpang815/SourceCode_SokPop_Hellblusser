using UnityEngine;

public class MoonWakeUp : MonoBehaviour
{
    public GameObject eyesClosedObject;
    public GameObject eyesOpenObject;

    int wakeUpWait, wakeUpCounter;
    bool wokeUp;
    public bool instantWakeUp;
    public bool onlyWakeWhenInteractionPressed;

    void Start ()
    {
        wakeUpWait = 210;
        wakeUpCounter = 0;

        if (!instantWakeUp)
        {
            eyesClosedObject.SetActive(true);
            eyesOpenObject.SetActive(false);
            wokeUp = false;
        }
        else
        {
            WakeUp();
        }
    }

    void LateUpdate ()
    {
        if (!onlyWakeWhenInteractionPressed)
        {
            if (!wokeUp)
            {
                if (wakeUpCounter < wakeUpWait)
                {
                    wakeUpCounter++;
                }
                else
                {
                    WakeUp();
                }
            }
        }
        else
        {
            eyesClosedObject.SetActive(!InputManager.instance.interactHold);
            eyesOpenObject.SetActive(InputManager.instance.interactHold);
        }
    }

    void WakeUp ()
    {
        eyesClosedObject.SetActive(false);
        eyesOpenObject.SetActive(true);
        wokeUp = true;
    }
}
