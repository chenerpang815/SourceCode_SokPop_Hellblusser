using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Sokpop
{
    public class SokIntro : MonoBehaviour
    {
        public static SokIntro instance;

        [HideInInspector] public int transitionDisableDur, transitionDisableCounter;

        public string NextSceneName;
        public Material PostProcessMat;

        public List<Sprite> SokSprites;
        public List<Sprite> HaarSprites;

        public CustomRenderPipelineCamera customPipelineCamera;
        public PostFXSettings customPipelineLoadingSettings;

        public List<AudioClip> SokpopSounds;

        public AudioSource toeterSource;
        bool playedToeter;
        int playToeterWait, playToeterCounter;

        public bool loading;

        public Image backgroundImage;
        public Image SokImage, HaarImage;
        public TextMeshProUGUI SokText, PopText;
        public TextMeshProUGUI loadText;

        private AudioSource audioSource;

        private bool sok, pop;
        private float angleRot, anglePlus, image_angle, image_xscale, image_yscale, scale, scaleTarg;
        private float t1, t2;

        private float haarRot, haarPlus, haar_xscale, haar_yscale;
        private int timer;

        private float image_index;

        private int popTimer;
        private int drawTimer;

        private float overgangRadius;

        void Awake ()
        {
            instance = this;

            loading = false;

            transitionDisableDur = 2;
            transitionDisableCounter = 0;

            playedToeter = false;
            playToeterWait = 40;
            playToeterCounter = 0;

            Application.targetFrameRate = 60;
            Draw();
            overgangRadius = 1f;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            scaleTarg = 1f;
            anglePlus = 5f;
        }

        private float fakeTimer;
        private void FixedUpdate()
        {
            fakeTimer += Time.deltaTime;

            while (fakeTimer >= 1f / 60f)
            {
                FakeUpdate();
                fakeTimer -= 1f / 60f;
            }
        }

        private void FakeUpdate()
        {
            // hide transition?
            if ( transitionDisableCounter < transitionDisableDur )
            {
                transitionDisableCounter++;
            }

            // toetertjeee?
            if (!playedToeter)
            {
                if (playToeterCounter < playToeterWait)
                {
                    playToeterCounter++;
                }
                else
                {
                    toeterSource.Play();
                    playedToeter = true;
                }
            }

            // opbokken met die cursor nou eens
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Draw();

            drawTimer++;
            if (drawTimer < 20)
            {
                return;
            }

            angleRot += anglePlus * 3f + 8f;
            anglePlus *= 0.95f;
            angleRot = angleRot % 360;

            image_angle = lengthdir_x(10f * anglePlus, angleRot) - 10f;

            scale += (scaleTarg - scale) * 0.15f;

            image_xscale = (1f + lengthdir_x(0.1f * anglePlus, angleRot)) * scale;
            image_yscale = (1f + lengthdir_y(0.1f * anglePlus, angleRot + 30f)) * scale;

            if (anglePlus < 0.4f && !sok)
            {
                audioSource.PlayOneShot(SokpopSounds[(int)TommieRandom.instance.RandomRange(0, SokpopSounds.Count)]);
                sok = true;
            }

            if (sok)
            {
                popTimer++;
                if (popTimer > 32)
                {
                    pop = true;
                }
            }


            if (sok && t1 < "sok".Length)
                t1 += 0.333334f;

            if (pop && t2 < "pop".Length)
                t2 += 0.333334f;


            haar_xscale = (1f + lengthdir_x(0.11f * anglePlus, angleRot - 30f)) * scale;
            haar_yscale = (1f + lengthdir_y(0.11f * anglePlus, angleRot - 10f)) * scale;

            timer++;

            bool next = (timer > 120);

            if (next)
            {
                overgangRadius -= 0.02f;

                if (overgangRadius < 0f)
                {
                    transitionDisableCounter = 0;
                    loading = true;
                    Draw();
                    customPipelineCamera.Settings.postFXSettings = customPipelineLoadingSettings;
                    SceneManager.LoadScene(NextSceneName);
                }
            }

            image_index += 0.1f;
            
        }

        private void Draw()
        {
            if ( loading )
            {
                SokText.enabled = false;
                PopText.enabled = false;

                SokImage.enabled = false;
                HaarImage.enabled = false;

                backgroundImage.enabled = false;

                string loadTextColA = "<#FFFFFF>";
                string loadTextColB = "<#69757F>";
                string loadTextString = loadTextColA + "loading" + "\n" + loadTextColB + "(takes a while)";
                loadText.text = loadTextString;
                loadText.enabled = true;
            }
            else
            {
                SokText.enabled = true;
                PopText.enabled = true;

                SokImage.enabled = true;
                HaarImage.enabled = true;

                backgroundImage.enabled = true;

                SokText.text = "sok".Substring(0, Mathf.FloorToInt(t1));
                PopText.text = "pop".Substring(0, Mathf.FloorToInt(t2));

                SokImage.transform.localScale = new Vector3(image_xscale, image_yscale, 1f);
                SokImage.transform.localEulerAngles = new Vector3(0f, 0f, image_angle);

                HaarImage.transform.localScale = new Vector3(haar_xscale, haar_yscale, 1f);
                HaarImage.transform.localEulerAngles = new Vector3(0f, 0f, image_angle);

                SokImage.sprite = SokSprites[Mathf.FloorToInt(image_index) % SokSprites.Count];
                HaarImage.sprite = HaarSprites[Mathf.FloorToInt(image_index) % HaarSprites.Count];

                loadText.enabled = false;
            }
        }

        private float lengthdir_x(float len, float dir)
        {
            return len * Mathf.Cos(Mathf.Deg2Rad * dir);
        }

        private float lengthdir_y(float len, float dir)
        {
            return len * Mathf.Sin(Mathf.Deg2Rad * dir);
        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            PostProcessMat.SetFloat("_CircleSize", overgangRadius);
            Graphics.Blit(source, destination, PostProcessMat);
        }
    }
}