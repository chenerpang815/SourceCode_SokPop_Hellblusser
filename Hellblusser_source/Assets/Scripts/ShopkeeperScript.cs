using UnityEngine;

public class ShopkeeperScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // eyes
    [Header("eyes")]
    public GameObject[] eyeNormalObjects;
    public GameObject[] eyeBlinkObjects;

    public enum EyeState { Normal, Blink, Dead };
    public EyeState curEyeState;

    // movement
    Vector3 posOriginal, posCur;

    // blink
    int blinkHoldDur, blinkHoldCounter, blinkRateMin, blinkRateMax, blinkRate, blinkRateCounter, blinkIndex;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // movement
        posOriginal = myTransform.position;
        posCur = posOriginal;

        // blink
        blinkIndex = 0;
        blinkHoldDur = 6;
        blinkHoldCounter = 0;

        blinkRateMin = 120;
        blinkRateMax = 360;
        blinkRate = Mathf.RoundToInt(TommieRandom.instance.RandomRange(blinkRateMin, blinkRateMax));
        blinkRateCounter = 0;
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze)
        {
            // eyes blinking
            if (blinkIndex == 0)
            {
                if (blinkRateCounter < blinkRate)
                {
                    blinkRateCounter++;
                }
                else
                {
                    blinkHoldCounter = 0;
                    blinkIndex = 1;
                }
            }
            else
            {
                if (blinkHoldCounter < blinkHoldDur)
                {
                    blinkHoldCounter++;
                }
                else
                {
                    blinkRateCounter = 0;
                    blinkRate = Mathf.RoundToInt(TommieRandom.instance.RandomRange(blinkRateMin, blinkRateMax));
                    blinkIndex = 0;
                }
            }

            switch (blinkIndex)
            {
                case 0: SetEyeState(EyeState.Normal); break;
                case 1: SetEyeState(EyeState.Blink); break;
            }

            // movement
            float moveLerpie = 2.5f;
            float moveRadius = .675f;
            Vector3 p0 = posOriginal;
            Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
            p1.y = p0.y;
            Vector3 p2 = (p1 - p0).normalized;
            Vector3 posTarget = posOriginal;
            posTarget += (p2 * moveRadius);

            float t0 = Time.time * 20f;
            float f0 = .25f;
            float s0 = Mathf.Sin(t0) * f0;
            posTarget.y += s0 * Vector3.Distance(posCur,posTarget);

            posCur = Vector3.Lerp(posCur,posTarget,moveLerpie * Time.deltaTime);
            myTransform.position = posCur;
        }
    }

    public void SetEyeState(EyeState _to)
    {
        for (int i = 0; i < 2; i++)
        {
            eyeNormalObjects[i].SetActive(_to == EyeState.Normal);
            eyeBlinkObjects[i].SetActive(_to == EyeState.Blink);
        }

        curEyeState = _to;
    }
}
