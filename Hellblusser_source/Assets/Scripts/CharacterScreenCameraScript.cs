using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScreenCameraScript : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;

    public Transform camPivotTransform;

    Vector3 posOriginal, posTarget, posCur;
    float scrollOff;

    Vector3 rotOriginal, rotTarget, rotCur;
    float hOff, vOff;

    float scrollOffExtra;

    public bool allowDebug;
    Vector3 posOff;

    float scrollOffMax;
    float scrollOffMin;

    void Start ()
    {
        myTransform = transform;

        // position
        posOriginal = myTransform.localPosition;
        posTarget = posOriginal;
        posCur = posTarget;

        // rotation
        rotOriginal = camPivotTransform.localRotation.eulerAngles;
        rotTarget = rotOriginal;
        rotCur = rotTarget;

        // scroll off extra
        scrollOffExtra = 125f;
        scrollOffMax = 2.5f;
        scrollOffMin = .5f;
        scrollOff = scrollOffMax * 1.25f;//6f;
    }

    void Update ()
    {
        if (!SetupManager.instance.paused)
        {
            bool allowInteraction = (allowDebug || (IntroManager.instance != null && IntroManager.instance.done));

            // position
            float horInput = InputManager.instance.moveDirection.x;
            float moveThreshold = .25f;


            float scrollInput = InputManager.instance.moveDirection.y; //0f;//Input.GetAxis("Mouse ScrollWheel");
            if (SetupManager.instance.inFreeze || !allowInteraction || Mathf.Abs(horInput) > moveThreshold)
            {
                scrollInput = 0f;
            }

            if (allowDebug)
            {
                float posOffSpd = 1f * Time.deltaTime;
                posOff.y += (InputManager.instance.moveDirection.x * posOffSpd);
            }

            /*
            if ( Input.GetKey(KeyCode.U) )
            {
                scrollInput = 1f;
            }
            else if ( Input.GetKey(KeyCode.I) )
            {
                scrollInput = -1f;
            }
            */

            float scrollSpd = .0375f;//.005f;
            scrollOff += (scrollInput * scrollSpd);
            //scrollOff = Mathf.Clamp(scrollOff,0f,scrollOffMax);

            if (allowInteraction)
            {
                float scrollFixSpd = .1f;
                if (scrollOff < scrollOffMin)
                {
                    scrollOff += Mathf.Abs((scrollOff - scrollOffMin) * scrollFixSpd);
                }
                if (scrollOff > scrollOffMax)
                {
                    scrollOff -= Mathf.Abs((scrollOffMax - scrollOff) * scrollFixSpd);
                }
            }

            scrollOffExtra = Mathf.Lerp(scrollOffExtra, 0f, 1f * Time.deltaTime);
            //scrollOff -= (scrollOffExtra * (scrollSpd * 2f));

            posTarget = posOriginal;
            posTarget.y -= scrollOff * .125f;
            posTarget.z += scrollOff * 1f;

            posCur = posTarget;//Vector3.Lerp(posCur,posTarget,20f * Time.deltaTime);
            myTransform.localPosition = posCur + posOff;

            /*
            // rotation
            Vector2 centerPos = new Vector2(Screen.width * .5f,Screen.height * .5f);
            Vector2 mousePos = centerPos + InputManager.instance.lookDirection; //GameManager.instance.mousePosition; //Input.mousePosition;

            float mouseFac = .05f;
            float hDiff = (centerPos.x - mousePos.x) * mouseFac;
            float vDiff = (centerPos.y - mousePos.y) * mouseFac * 2f;

            float rotSpd = .1f;
            float hSpd = rotSpd;
            float vSpd = rotSpd;

            rotTarget = rotOriginal;

            float maxOff = 5f;

            hOff = (hDiff * hSpd);
            vOff = -(vDiff * vSpd);

            hOff = Mathf.Clamp(hOff,-maxOff,maxOff);
            vOff = Mathf.Clamp(vOff,-maxOff,maxOff);

            rotTarget.x += vOff;
            rotTarget.y += hOff;
            */

            float lookSpd = .1f;
            hOff = InputManager.instance.lookDirection.x;
            vOff = InputManager.instance.lookDirection.y;

            if (SetupManager.instance.inFreeze)
            {
                hOff = 0f;
                vOff = 0f;
            }

            if (allowInteraction)
            {
                rotTarget.x -= (vOff * lookSpd);
                rotTarget.y += (hOff * lookSpd);
            }

            float maxLookOff = 6.25f;
            rotTarget.x = Mathf.Clamp(rotTarget.x, -maxLookOff, maxLookOff);
            rotTarget.y = Mathf.Clamp(rotTarget.y, -maxLookOff, maxLookOff);

            rotCur = Vector3.Lerp(rotCur, rotTarget, 20f * Time.deltaTime);
            camPivotTransform.localRotation = Quaternion.Euler(rotCur);
        }
    }
}
