using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // instance
    public static AudioManager instance;

    // clips
    [Header("footstep")]
    public AudioClip[] footstepDefaultClips;

    [Header("combat")]
    public AudioClip[] fireMagicCastClips;
    public AudioClip[] fireMagicImpactClips;
    public AudioClip[] meleeAttackClips;
    public AudioClip[] magicHitImpactClips;
    public AudioClip[] meleeHitImpactClips;
    public AudioClip[] enemyClearClips;
    public AudioClip[] magicChargeClips;
    public AudioClip[] magicReadyClips;
    public AudioClip[] magicCollectClips;
    public AudioClip[] playerHurtClips;
    public AudioClip[] playerDeadClips;
    public AudioClip[] meleeBlockedClips;
    public AudioClip[] kickDoClips;
    public AudioClip[] kickHitImpactClips;
    public AudioClip[] magicStartChargeClips;

    [Header("UI")]
    public AudioClip[] UINavigateClips;
    public AudioClip[] UISelectClips;
    public AudioClip[] UILevelSelectClips;
    public AudioClip[] UIBackClips;
    public AudioClip[] popupOpenClips;
    public AudioClip[] popupCloseClips;

    [Header("rat")]
    public AudioClip[] ratAlertClips;
    public AudioClip[] ratAttackPrepareClips, ratAttackDoClips;
    public AudioClip[] ratHurtClips;
    public AudioClip[] ratDeadClips;

    [Header("fire")]
    public AudioClip[] fireStartClips;
    public AudioClip[] fireStopClips;

    [Header("misc")]
    public AudioClip[] clayBreakClips;
    public AudioClip[] woodBreakClips;
    public AudioClip[] metalImpactClips;
    public AudioClip[] metalBreakClips;
    public AudioClip[] stoneImpactClips;
    public AudioClip[] stoneBreakClips;
    public AudioClip[] coinImpactClips;
    public AudioClip[] coinCollectClips;
    public AudioClip[] tearImpactClips;
    public AudioClip[] tearCollectClips;
    public AudioClip[] donutImpactClips;
    public AudioClip[] donutCollectClips;
    public AudioClip[] boneImpactClips;
    public AudioClip[] playerFireAddClips;
    public AudioClip[] clearedAllFireClips;
    public AudioClip[] foundKeyClips;
    public AudioClip[] faerieAppearClips;
    public AudioClip[] faerieLaughClips;
    public AudioClip[] faerieCastSpellClips;
    public AudioClip[] faerieMagicImpactClips;
    public AudioClip[] cannonLightClips;
    public AudioClip[] cannonShootClips;
    public AudioClip[] cannonImpactClips;
    public AudioClip[] bossDefeatedClips;
    public AudioClip[] spawnMinionClips;
    public AudioClip[] stairsDescendClips;
    public AudioClip[] shopBuyItemClips;
    public AudioClip[] restClips;
    public AudioClip[] playerSetDeadClips;
    public AudioClip[] countdownProceedClips;
    public AudioClip[] countdownEndClips;
    public AudioClip[] fountainFillClips;
    public AudioClip[] blessingPickClips;
    public AudioClip[] playerHealClips;
    public AudioClip[] magicSkullAwokenClips;

    [Header("environment")]
    public AudioClip[] gateOpen;
    public AudioClip[] gateClosed;
    public AudioClip[] doorOpen;
    public AudioClip[] doorClosed;

    // magic collect source
    [Header("magic collect")]
    public AudioSource magicCollectAudioSource;

    // ambience
    [Header("ambience")]
    public AudioSource fireAmbienceSource;
    public AudioSource playerBurningAmbienceSource;
    public AudioSource lavaClearingAmbienceSource;

    // global audio sources
    int globalAudioSourceCount;
    List<AudioSource> globalAudioSources;

    // dynamic 3d audio sources
    int dynamicAudioSourceCount;
    List<AudioSource> dynamicAudioSources;
    List<Transform> dynamicAudioSourceTransforms;
    List<GameObject> dynamicAudioSourceGameObjects;

    void Awake ()
    {
        instance = this;

        // global audioSources
        globalAudioSourceCount = 10;
        globalAudioSources = new List<AudioSource>();
        for (int i = 0; i < globalAudioSourceCount; i++)
        {
            GameObject audioSourceO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.audioSource2D, Vector3.zero, Quaternion.identity, 1f);
            Transform audioSourceTr = audioSourceO.transform;
            audioSourceTr.parent = SetupManager.instance.myTransform;
            AudioSource audioSource = audioSourceO.GetComponent<AudioSource>();
            globalAudioSources.Add(audioSource);
        }

        // dynamic audioSources
        dynamicAudioSourceCount = 10;
        dynamicAudioSourceTransforms = new List<Transform>();
        dynamicAudioSourceGameObjects = new List<GameObject>();
        dynamicAudioSources = new List<AudioSource>();
        for (int i = 0; i < dynamicAudioSourceCount; i++)
        {
            GameObject audioSourceO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.audioSource3D, Vector3.zero, Quaternion.identity, 1f);
            Transform audioSourceTr = audioSourceO.transform;
            audioSourceTr.parent = SetupManager.instance.myTransform;
            AudioSource audioSource = audioSourceO.GetComponent<AudioSource>();
            dynamicAudioSourceTransforms.Add(audioSourceTr);
            dynamicAudioSourceGameObjects.Add(audioSourceO);
            dynamicAudioSources.Add(audioSource);
        }
    }

    void Update ()
    {
        if (SetupManager.instance != null && !SetupManager.instance.inFreeze)
        {
            // fire ambience
            if (fireAmbienceSource != null)
            {
                float fireAmbienceLerpie = 2.5f;
                float fireAmbienceVolTarget = 0f;
                if (SetupManager.instance.playerNearFire)
                {
                    fireAmbienceVolTarget = (.325f * SetupManager.instance.sfxVolFactor);
                }
                if (SetupManager.instance.curGameState != SetupManager.GameState.Level && SetupManager.instance.curGameState != SetupManager.GameState.Shop && SetupManager.instance.curGameState != SetupManager.GameState.Rest)
                {
                    fireAmbienceVolTarget = 0f;
                }
                fireAmbienceSource.volume = Mathf.Lerp(fireAmbienceSource.volume, fireAmbienceVolTarget, fireAmbienceLerpie * Time.deltaTime);
            }

            // player burning
            if (playerBurningAmbienceSource != null)
            {
                float playerBurningAmbienceLerpie = 2.5f;
                float playerBurningAmbienceVolTarget = 0f;
                if (SetupManager.instance.playerBurning)
                {
                    playerBurningAmbienceVolTarget = (.625f * SetupManager.instance.sfxVolFactor);
                }
                if (SetupManager.instance.curGameState != SetupManager.GameState.Level && SetupManager.instance.curGameState != SetupManager.GameState.Shop && SetupManager.instance.curGameState != SetupManager.GameState.Rest)
                {
                    playerBurningAmbienceVolTarget = 0f;
                }
                playerBurningAmbienceSource.volume = Mathf.Lerp(playerBurningAmbienceSource.volume, playerBurningAmbienceVolTarget, playerBurningAmbienceLerpie * Time.deltaTime);
            }

            // lava clearing
            if (lavaClearingAmbienceSource != null)
            {
                float lavaClearingAmbienceLerpie = 2.5f;
                float lavaClearingAmbienceVolTarget = 0f;
                if (SetupManager.instance.clearingLava)
                {
                    lavaClearingAmbienceVolTarget = (.325f * SetupManager.instance.sfxVolFactor);
                }
                if (SetupManager.instance.curGameState != SetupManager.GameState.Level && SetupManager.instance.curGameState != SetupManager.GameState.Shop && SetupManager.instance.curGameState != SetupManager.GameState.Rest)
                {
                    lavaClearingAmbienceVolTarget = 0f;
                }
                lavaClearingAmbienceSource.volume = Mathf.Lerp(lavaClearingAmbienceSource.volume, lavaClearingAmbienceVolTarget, lavaClearingAmbienceLerpie * Time.deltaTime);
            }
        }
    }

    public void PlaySound ( AudioSource _source, AudioClip _clip, float _pMin, float _pMax, float _vMin, float _vMax )
    {
        if ( _source != null && _clip != null )
        {
            _source.pitch = TommieRandom.instance.RandomRange(_pMin,_pMax);
            _source.volume = TommieRandom.instance.RandomRange(_vMin,_vMax) * SetupManager.instance.sfxVolFactor;
        }
    }

    public void PlaySoundGlobal(AudioClip _clip, float _pMin, float _pMax, float _vMin, float _vMax)
    {
        //Debug.Log("try to play " + _clip + " || " + Time.time.ToString());

        AudioSource sourceUse = null;
        for (int i = 0; i < globalAudioSourceCount; i++)
        {
            AudioSource sourceCheck = globalAudioSources[i];
            if (sourceCheck != null && !sourceCheck.isPlaying)
            {
                sourceUse = sourceCheck;
                break;
            }
        }
        if (sourceUse != null)
        {
            sourceUse.pitch = TommieRandom.instance.RandomRange(_pMin,_pMax);
            sourceUse.volume = TommieRandom.instance.RandomRange(_vMin,_vMax) * SetupManager.instance.sfxVolFactor;
            sourceUse.PlayOneShot(_clip);
        }
    }

    public void PlaySoundAtPosition ( Vector3 _pos, AudioClip _clip, float _pMin, float _pMax, float _vMin, float _vMax, float _minDistance = 2f, float _maxDistance = 8f )
    {
        AudioSource sourceUse = null;
        Transform sourceTransformUse = null;
        for ( int i = 0; i < dynamicAudioSourceCount; i ++ )
        {
            AudioSource sourceCheck = dynamicAudioSources[i];
            if ( sourceCheck != null && !sourceCheck.isPlaying )
            {
                sourceUse = sourceCheck;
                sourceUse.minDistance = _minDistance;
                sourceUse.maxDistance = _maxDistance;
                sourceTransformUse = dynamicAudioSourceTransforms[i];
                break;
            }
        }
        if ( sourceUse != null )
        {
            sourceTransformUse.position = _pos;

            sourceUse.pitch = TommieRandom.instance.RandomRange(_pMin,_pMax);
            sourceUse.volume = TommieRandom.instance.RandomRange(_vMin,_vMax) * SetupManager.instance.sfxVolFactor;
            sourceUse.PlayOneShot(_clip);
        }

        //Debug.Log("play sound at position || clip: " + _clip + " || source: " + sourceUse + " || " + Time.time.ToString());
    }

    public void PlayDoorClosedSound()
    {
        PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(doorClosed), .6f, .9f, .3f, .325f);
    }

    public void PlayDoorOpenSound()
    {
        PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(doorOpen), .6f, .9f, .3f, .325f);
    }

    public void PlayAttackImpactSound ( Npc.AttackData.DamageType _type )
    {
        switch (_type)
        {
            case Npc.AttackData.DamageType.Melee: PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(meleeHitImpactClips), .6f, .9f, .225f, .25f); break;
            case Npc.AttackData.DamageType.Magic: PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(magicHitImpactClips), .6f, .9f, .225f, .25f); break;
        }
    }
}
