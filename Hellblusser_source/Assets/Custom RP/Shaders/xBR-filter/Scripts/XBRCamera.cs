using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class XBRCamera : MonoBehaviour
{
    public Shader xBR;
    public Shader xBR1;
    public Shader xBR2;
    public Shader xBR_sharper;

    public int width, height;

    public const int scale = 2;

    Material[] materials = new Material[4];
    RenderTexture[] renderTextures = new RenderTexture[3];

    public bool xbrEnabled = true;

    bool resolutionChanged;
    int widthPrev, heightPrev;

    void Start()
    {
        //Screen.SetResolution(1280,800,true);

        if ( !Application.isEditor )
        {
            xbrEnabled = true;
        }

        UpdateResolution();
    }

    public void UpdateResolution ()
    {
        Resolution curResolution = Screen.currentResolution;

        float sclFac = .575f;

        float widthBase = 1280f;
        float heightBase = 720f;

        float originalAspect = (widthBase / heightBase);

        float widthNew = curResolution.width;
        float heightNew = curResolution.height;
        float newAspect = (widthNew / heightNew);

        //float leukeScale = newAspect / originalAspect;

        float leukeScale = 1f / Mathf.Min((float)(curResolution.width) / widthBase, (float)(curResolution.height) / heightBase);

        leukeScale *= sclFac;

        //Debug.Log("curResolution: " + curResolution.ToString() + " || leuke scale " + leukeScale.ToString() + " || " + Time.time.ToString());

        int widthRead = Mathf.FloorToInt((float)(curResolution.width) * leukeScale);
        int heightRead = Mathf.FloorToInt((float)(curResolution.height) * leukeScale);

        int div = 2;
        width = (widthRead / div) * div;
        height = (heightRead / div) * div;

        widthPrev = curResolution.width;
        heightPrev = curResolution.height;

        RenderTexture t = new RenderTexture(width / scale, height / scale, 24, RenderTextureFormat.Default);
        t.hideFlags = HideFlags.HideAndDontSave;
        t.filterMode = FilterMode.Point;

        Camera.main.targetTexture = t;
        //Camera.main.orthographicSize = height / (2f * scale);
        //GameManager.instance.mainCameraScript.myCam.

        Vector2 screenDimensions = new Vector2(width, height);

        materials[0] = CreateMaterial(xBR, screenDimensions / 2f);
        materials[1] = CreateMaterial(xBR1, screenDimensions);
        materials[2] = CreateMaterial(xBR2, screenDimensions);
        materials[3] = CreateMaterial(xBR_sharper, screenDimensions);

        for (int i = 0; i < renderTextures.Length; i++)
        {
            renderTextures[i] = CreateRenderTexture();
        }
    }

    Material CreateMaterial(Shader shader,Vector2 screenDimensions)
    {
        Material mat = new Material(shader);

        mat.hideFlags = HideFlags.HideAndDontSave;
        mat.SetVector("_TextureSizeVector", new Vector4(screenDimensions.x,screenDimensions.y,0f,0f));

        return mat;
    }

    RenderTexture CreateRenderTexture()
    {
        RenderTexture rt;

        rt = new RenderTexture(width,height,24,RenderTextureFormat.Default);
        rt.hideFlags = HideFlags.HideAndDontSave;
        rt.filterMode = FilterMode.Point;
        rt.anisoLevel = 0;
        rt.Create();

        return rt;
    }

    void Update()
    {
        if ( Application.isEditor )
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                xbrEnabled = !xbrEnabled;
            }
        }

        //Resolution curResolution = Screen.currentResolution;
        //if ( Input.GetKeyDown(KeyCode.F) || (curResolution.width != widthPrev || curResolution.height != heightPrev) )
        //{
        //    UpdateResolution();

        //    // debug
        //    //Debug.Log("updated screen size || " + Time.time.ToString());
        //}
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (xbrEnabled)
        {
            Graphics.Blit(Camera.main.targetTexture, renderTextures[0], materials[0]);
            Graphics.Blit(renderTextures[0], renderTextures[1], materials[1]);
            Graphics.Blit(renderTextures[1], renderTextures[2], materials[2]);
            Graphics.Blit(renderTextures[2], dest, materials[3]);
        }
        else
        {
            Graphics.Blit(Camera.main.targetTexture, dest);
        }
    }
}
