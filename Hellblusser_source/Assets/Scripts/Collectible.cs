using UnityEngine;

public class Collectible : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // type
    public enum Type { Coin, Tear, Key, Donut };

    // settings
    [Header("settings")]
    public Type myType;
    public int value;
    public float radius;

    // lerpie
    float lerpieAdd;

    // counter
    int canCollectDur, canCollectCounter;

    // audio
    [Header("audio")]
    public float pitchMin;
    public float pitchMax;
    public float volumeMin;
    public float volumeMax;

    // state
    [HideInInspector] public bool collected;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;

        canCollectDur = 16;
        if ( myType == Type.Key )
        {
            canCollectDur = 24;
        }
        canCollectCounter = 0;
    }

    void Update ()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            if (!collected)
            {
                if (canCollectCounter < canCollectDur)
                {
                    canCollectCounter++;
                }
                else
                {
                    Vector3 p0 = myTransform.position;
                    Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                    float d0 = Vector3.Distance(p0, p1);
                    if (d0 <= radius)
                    {
                        Collect();
                    }
                }
            }
            else
            {
                lerpieAdd += .05f;
                Vector3 targetP = GameManager.instance.playerFirstPersonDrifter.myTransform.position + (Vector3.up * .5f);
                myTransform.position = Vector3.Lerp(myTransform.position, targetP, (10f + lerpieAdd) * Time.deltaTime);

                float d1 = Vector3.Distance(myTransform.position, targetP);
                if (d1 <= .25f)
                {
                    Claim();
                }
            }
        }
    }

    public void Collect ()
    {
        collected = true;
    }

    public void Claim ()
    {
        // audio
        AudioClip[] clipsUse = null;
        switch (myType)
        {
            case Type.Coin: GameManager.instance.AddCoinAmount(value); clipsUse = AudioManager.instance.coinCollectClips; GameManager.instance.coinCollected = true; break;
            case Type.Tear: GameManager.instance.AddTearAmount(value); clipsUse = AudioManager.instance.tearCollectClips; GameManager.instance.tearCollected = true; break;
            case Type.Key: GameManager.instance.PlayerFoundKey(); clipsUse = AudioManager.instance.coinCollectClips; GameManager.instance.keyCollected = true; break;
            case Type.Donut: GameManager.instance.AddPlayerHealth(1); clipsUse = AudioManager.instance.donutCollectClips; GameManager.instance.donutCollected = true; break;
        }
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(clipsUse),pitchMin,pitchMax,volumeMin,volumeMax);

        // donut achievement?
        if ( myType == Type.Donut && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Hungry) )
        {
            AchievementHelper.UnlockAchievement("ACHIEVEMENT_SATISFIED");
        }

        // doeg
        Clear();
    }

    public void Clear ()
    {
        // particles
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position, Quaternion.identity, .5f);

        // oke dag toedeledokie
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }
}
