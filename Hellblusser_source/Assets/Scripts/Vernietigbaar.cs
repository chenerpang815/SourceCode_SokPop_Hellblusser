using UnityEngine;
using System.Collections.Generic;

public class Vernietigbaar : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // references
    [Header("references")]
    public FlameableScript myFlameableScript;

    // settings
    [Header("settings")]
    public float radius;
    public int coinDropCount;
    public Vector3 hitOffset;
    public float whiteOrbScale;

    // drop type
    public enum DropType { Coin, Tear, Key, Dust, Donut };

    [System.Serializable]
    public struct DropData
    {
        public DropType dropType;
        public int weight;
    }

    public List<DropData> possibleDropDatas;
    [HideInInspector] public DropType myDropType;

    // audio
    public enum Type { Wood, Clay, Stone, Metal, Bone };

    [Header("audio")]
    public Type myType;
    public float pitchMin;
    public float pitchMax;
    public float volumeMin;
    public float volumeMax;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;

        // define drop type
        WeightedRandomBag<DropType> possibleDropTypes = new WeightedRandomBag<DropType>();
        for ( int i = 0; i < possibleDropDatas.Count; i ++ )
        {
            DropData curDropData = possibleDropDatas[i];
            possibleDropTypes.AddEntry(curDropData.dropType,curDropData.weight);
        }
        if (SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Hungry) && (TommieRandom.instance.RandomValue(1f) <= .1f) )
        {
            possibleDropTypes.AddEntry(DropType.Donut,4);
        }
        myDropType = possibleDropTypes.Choose();

        // lucky blessing
        if ( myDropType == DropType.Coin && (SetupManager.instance != null && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Lucky)) )
        {
            if ( TommieRandom.instance.RandomValue(1f) <= .33f )
            {
                coinDropCount++;
            }
        }

        // store
        Store();
    }

    void Store ()
    {
        GameManager.instance.allVernietigbaarScripts.Add(this);
    }

    public void Destroy ()
    {
        // waar gaan die dingen droppen
        switch (myDropType)
        {
            case DropType.Coin:
                for (int i = 0; i < coinDropCount; i++)
                {
                    GameObject coinO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.coinPrefab[0], myTransform.position + hitOffset, Quaternion.identity, 1f);
                    Stuiterbaar stuiterbaarScript = coinO.GetComponent<Stuiterbaar>();
                    if (stuiterbaarScript != null)
                    {
                        float spawnSideForceOffMax = .05f;
                        float spawnUpForceOffMax = .075f;
                        float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                        float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                        float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                        float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                        stuiterbaarScript.forceCur.x += (xAdd * xDir);
                        stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                        stuiterbaarScript.forceCur.z += (zAdd * zDir);
                    }
                }
            break;

            case DropType.Key:
                for (int i = 0; i < 1; i++)
                {
                    GameObject keyO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.keyPrefab[0], myTransform.position + hitOffset, Quaternion.identity, 1f);
                    Stuiterbaar stuiterbaarScript = keyO.GetComponent<Stuiterbaar>();
                    if (stuiterbaarScript != null)
                    {
                        float spawnSideForceOffMax = .05f;
                        float spawnUpForceOffMax = .075f;
                        float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                        float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                        float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                        float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                        stuiterbaarScript.forceCur.x += (xAdd * xDir);
                        stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                        stuiterbaarScript.forceCur.z += (zAdd * zDir);
                    }
                }
                break;

            case DropType.Dust:
                PrefabManager.instance.SpawnPrefab(PrefabManager.instance.dustImpactParticlesPrefab[0], myTransform.position + hitOffset, Quaternion.identity, 1f);
                AudioManager.instance.PlaySoundAtPosition(myTransform.position + hitOffset,BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireMagicImpactClips),.6f,.9f,.2f,.225f);
                break;

            case DropType.Donut:
                int donutCount = 1;
                for (int i = 0; i < donutCount; i++)
                {
                    GameObject donutO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.donutPrefab[0], myTransform.position + hitOffset, Quaternion.identity, 1f);
                    Stuiterbaar stuiterbaarScript = donutO.GetComponent<Stuiterbaar>();
                    if (stuiterbaarScript != null)
                    {
                        float spawnSideForceOffMax = .05f;
                        float spawnUpForceOffMax = .075f;
                        float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                        float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                        float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                        float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                        stuiterbaarScript.forceCur.x += (xAdd * xDir);
                        stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                        stuiterbaarScript.forceCur.z += (zAdd * zDir);
                    }
                }
                break;
        }

        // particles
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position + hitOffset, Quaternion.identity, whiteOrbScale);
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[2], myTransform.position + hitOffset, Quaternion.identity, 1f * whiteOrbScale);

        // audio
        AudioClip[] clipsUse = null;
        switch ( myType )
        {
            case Type.Wood: clipsUse = AudioManager.instance.woodBreakClips; break;
            case Type.Clay: clipsUse = AudioManager.instance.clayBreakClips; break;
            case Type.Stone: clipsUse = AudioManager.instance.stoneBreakClips; break;
            case Type.Metal: clipsUse = AudioManager.instance.metalBreakClips; break;
        }
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(clipsUse),pitchMin,pitchMax,volumeMin,volumeMax);

        // clear fire?
        if ( myFlameableScript != null && myFlameableScript.myFireScript != null )
        {
            myFlameableScript.myFireScript.Clear();
        }

        // doei druif
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + hitOffset,radius);
    }
}
