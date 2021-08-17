using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RenderTextureCanvas : MonoBehaviour 
{
    public RawImage rawImage;

    [HideInInspector] public RenderTexture rt;

    [HideInInspector] public GameObject visualsObject;
    [HideInInspector] public Transform visualsTransform;

    public Camera camToUse;

    public int w;
    public int h;

    public bool useScreenSize;
    public float screenSizeRenderScale;

    public bool autoInit;

    public bool autoUpdate;

    public bool autoEnable;

    public bool dontClearCamera;

    int wPrev, hPrev;

    public Texture2D imageRead;

    [HideInInspector] public bool rotateVisuals;
    [HideInInspector] public float visualsRotateSpeed;
    [HideInInspector] public float visualsRotateAmplitude;
    [HideInInspector] public Quaternion visualsRotOriginal;

    private void Awake()
    {
        if (autoInit)
        {
            Init();
        }
    }

    public void Init () 
	{
        if (rawImage != null)
        {
            CreateRenderTexture();
        }

        if ( visualsTransform != null )
        {
            visualsRotOriginal = visualsTransform.localRotation;
        }
    }

    private void Update()
    {
        if ( autoUpdate && rt != null && rt.IsCreated() )
        {
            if ( Screen.width != wPrev || Screen.height != hPrev )
            {
                CreateRenderTexture();
            }
        }

        if ( rotateVisuals && visualsTransform != null )
        {
            float t0 = Time.time * visualsRotateSpeed;
            float f0 = visualsRotateAmplitude;
            float s0 = Mathf.Sin(t0) * f0;
            visualsTransform.localRotation = visualsRotOriginal * Quaternion.Euler(0f,s0,0f);
        }
    }

    public void Clear ()
    {
        if ( rt != null )
        {
            rt.DiscardContents(true,true);
            rt.Release();
        }
    }

    void CreateRenderTexture ()
	{
        if (rawImage != null)
        {
            rawImage.enabled = false;

            Clear();

            int ww = (useScreenSize) ? Mathf.CeilToInt((float)Screen.width * screenSizeRenderScale) : w;
            int hh = (useScreenSize) ? Mathf.CeilToInt((float)Screen.height * screenSizeRenderScale) : h;
            rt = new RenderTexture(ww, hh, 24, RenderTextureFormat.ARGB32);
            rt.useMipMap = false;
            rt.autoGenerateMips = false;
            rt.filterMode = FilterMode.Point;
            rt.name = "renderTargetTexture";
            rt.anisoLevel = 0;

            if (!rt.IsCreated())
            {
                rt.Create();
            }

            wPrev = ww;
            hPrev = hh;

            // apply
            StartCoroutine(ApplyTexture());
        }
	}

	IEnumerator ApplyTexture ()
	{
        if (rawImage != null)
        {
            rawImage.enabled = false;

            yield return new WaitUntil(() => rt.IsCreated());

            if ( !autoUpdate )
            {
                if (!dontClearCamera)
                {
                    imageRead = BasicFunctions.RTImage(rt.width, rt.height, camToUse);

                    rawImage.texture = imageRead;

                    ClearCamera();
                }
                else
                {
                    camToUse.targetTexture = rt;

                    rawImage.texture = camToUse.targetTexture;
                }
            }
            else
            {
                camToUse.targetTexture = rt;

                rawImage.texture = rt;
            }

            rawImage.enabled = true;
        }
    }

    public void ClearCamera ()
    {
        Destroy(this);

        if (camToUse != null)
        {
            Destroy(camToUse.gameObject);
        }

        if ( visualsObject != null )
        {
            Destroy(visualsObject);
        }
    }

    void OnApplicationQuit ()
    {
        Clear();
    }
}
