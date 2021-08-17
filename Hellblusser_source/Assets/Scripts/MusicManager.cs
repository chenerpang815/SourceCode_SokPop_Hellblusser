using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    // instance
    public static MusicManager instance;

    // music structures
    [Header("music structures")]
    public MusicStructure combatStructure0;
    public MusicStructure combatStructure1;
    public MusicStructure combatStructure2;
    public MusicStructure combatStructure3;
    public MusicStructure combatStructure4;
    public MusicStructure combatStructure5;
    public MusicStructure combatStructure6;
    public MusicStructure combatStructure7;
    public MusicStructure combatStructure8;
    public MusicStructure bossCombatStructure0;
    public MusicStructure bossCombatStructure1;

    // source
    [Header("audioSource")]
    public AudioSource[] musicAudioSources;

    // state
    [HideInInspector] public bool running;
    public enum StructureState { Idle, Low, Medium, High };
    public StructureState curStructureState;
    [HideInInspector] public StructureState volumeStructureStateRead;
    int structureStateIndex;
    [HideInInspector] public MusicStructure curMusicStructure;
    int combatStopDur, combatStopCounter;

    // structure
    MusicStructure musicStructurePlay;

    // target
    public enum MusicTarget { Active, Inactive };
    public MusicTarget curMusicTarget;

    // play data
    double nextEventTime;
    int flip;

    void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        flip = 0;

        combatStopDur = 180;
        combatStopCounter = combatStopDur;

        running = false;
    }

    void Update ()
    {
        // check current structure
        CheckCurrentStructure();
    }

    public void StartMusicStructure ()
    {
        musicStructurePlay = combatStructure0;
        switch (SetupManager.instance.runDataRead.curFloorIndex)
        {
            // SEWER
            case 1: DefineSewerMusic(); break;

            // DUNGEON
            case 2: DefineDungeonMusic(); break;

            // HELL
            case 3: DefineHellMusic(); break;
        }

        nextEventTime = AudioSettings.dspTime;

        structureStateIndex = -1;
        SetStructureState(StructureState.Idle);
        SetMusicStructure(musicStructurePlay);
        SetMusicTarget(MusicTarget.Active);

        running = true;

        // log
        //Debug.Log("start music structure || " + Time.time.ToString());
    }

    public void DefineSewerMusic ()
    {
        int encountersHadCheck = (SetupManager.instance.runDataRead.encountersHad + 1);
        if (SetupManager.instance.curEncounterType != SetupManager.EncounterType.Boss)
        {
            switch (encountersHadCheck)
            {
                case 1: musicStructurePlay = combatStructure1; break;
                case 2: musicStructurePlay = combatStructure0; break;
                case 3: musicStructurePlay = combatStructure2; break;
            }
            if (encountersHadCheck > 3)
            {
                WeightedRandomBag<MusicStructure> temp = new WeightedRandomBag<MusicStructure>();
                temp.AddEntry(combatStructure0,10);
                temp.AddEntry(combatStructure1,10);
                temp.AddEntry(combatStructure2,10);
                musicStructurePlay = temp.Choose();
            }
        }
        else
        {
            List<MusicStructure> bossMusicStructuresPossible = new List<MusicStructure>();
            bossMusicStructuresPossible.Add(bossCombatStructure0);

            if (bossMusicStructuresPossible.Count > 0)
            {
                int bossMusicIndex = Mathf.RoundToInt(TommieRandom.instance.RandomRange(0f, bossMusicStructuresPossible.Count - 1));
                musicStructurePlay = bossMusicStructuresPossible[bossMusicIndex];
            }
        }
    }

    public void DefineDungeonMusic ()
    {
        int encountersHadCheck = (SetupManager.instance.runDataRead.encountersHad + 1);
        if (SetupManager.instance.curEncounterType != SetupManager.EncounterType.Boss)
        {
            switch (encountersHadCheck)
            {
                case 1: musicStructurePlay = combatStructure3; break;
                case 2: musicStructurePlay = combatStructure4; break;
                case 3: musicStructurePlay = combatStructure5; break;
            }
            if (encountersHadCheck > 3)
            {
                WeightedRandomBag<MusicStructure> temp = new WeightedRandomBag<MusicStructure>();
                temp.AddEntry(combatStructure3, 10);
                temp.AddEntry(combatStructure4, 10);
                temp.AddEntry(combatStructure5, 10);
                musicStructurePlay = temp.Choose();
            }
        }
        else
        {
            List<MusicStructure> bossMusicStructuresPossible = new List<MusicStructure>();
            bossMusicStructuresPossible.Add(bossCombatStructure0);

            if (bossMusicStructuresPossible.Count > 0)
            {
                int bossMusicIndex = Mathf.RoundToInt(TommieRandom.instance.RandomRange(0f, bossMusicStructuresPossible.Count - 1));
                musicStructurePlay = bossMusicStructuresPossible[bossMusicIndex];
            }
        }
    }

    public void DefineHellMusic ()
    {
        int encountersHadCheck = (SetupManager.instance.runDataRead.encountersHad + 1);

        if (SetupManager.instance.curEncounterType != SetupManager.EncounterType.Boss)
        {
            switch (encountersHadCheck)
            {
                case 1: musicStructurePlay = combatStructure6; break;
                case 2: musicStructurePlay = combatStructure7; break;
                case 3: musicStructurePlay = combatStructure8; break;
            }
            if (encountersHadCheck > 3)
            {
                WeightedRandomBag<MusicStructure> temp = new WeightedRandomBag<MusicStructure>();
                temp.AddEntry(combatStructure6, 10);
                temp.AddEntry(combatStructure7, 10);
                temp.AddEntry(combatStructure8, 10);
                musicStructurePlay = temp.Choose();
            }
        }
        else
        {
            List<MusicStructure> bossMusicStructuresPossible = new List<MusicStructure>();
            bossMusicStructuresPossible.Add(bossCombatStructure1);

            if (bossMusicStructuresPossible.Count > 0)
            {
                int bossMusicIndex = Mathf.RoundToInt(TommieRandom.instance.RandomRange(0f, bossMusicStructuresPossible.Count - 1));
                musicStructurePlay = bossMusicStructuresPossible[bossMusicIndex];
            }
        }
    }

    public void StopMusicStructure ()
    {
        structureStateIndex = -1;
        SetStructureState(StructureState.Idle);
        for ( int i = 0; i < musicAudioSources.Length; i ++ )
        {
            musicAudioSources[i].volume = Mathf.Lerp(musicAudioSources[i].volume,0f,1f * Time.deltaTime);
        }
        running = false;
    }

    void CheckCurrentStructure ()
    {
        bool playMusicSystem = true;
        if (SetupManager.instance.curGameState != SetupManager.GameState.Level && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick )
        {
            playMusicSystem = false;
        }
        if ( GameManager.instance != null && GameManager.instance.inGameWait )
        {
            playMusicSystem = false;
        }

        if (playMusicSystem)
        {
            if (!running)
            {
                StartMusicStructure();
            }
            else if ( GameManager.instance != null )
            {
                double time = AudioSettings.dspTime;

                int structureStateIndexMax = 0;
                StructureState nextStructureState;
                switch (curStructureState)
                {
                    default: nextStructureState = StructureState.Low; break;
                    case StructureState.Idle: structureStateIndexMax = (curMusicStructure.clipIdle.Length - 1); nextStructureState = StructureState.Low; break;
                    case StructureState.Low: structureStateIndexMax = (curMusicStructure.clipLow.Length - 1); nextStructureState = StructureState.Medium; break;
                    case StructureState.Medium: structureStateIndexMax = (curMusicStructure.clipMedium.Length - 1); nextStructureState = StructureState.High; break;
                    case StructureState.High: structureStateIndexMax = (curMusicStructure.clipHigh.Length - 1); nextStructureState = StructureState.Medium; break;
                }

                // not yet in combat, so keep looping the idle part
                if ( !GameManager.instance.playerInCombat && curStructureState == StructureState.Idle )
                {
                    nextStructureState = StructureState.Idle;
                }

                // volume
                float volTarget;// = curMusicStructure.volume;
                if (SetupManager.instance.curGameState != SetupManager.GameState.Level && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick)
                {
                    volTarget = 0f;
                }
                else
                {
                    if (GameManager.instance.playerInCombat)
                    {
                        combatStopCounter = 0;
                        volTarget = curMusicStructure.volume;
                    }
                    else
                    {
                        if (combatStopCounter < combatStopDur)
                        {
                            combatStopCounter++;
                            volTarget = curMusicStructure.volume;
                        }
                        else
                        {
                            if (volumeStructureStateRead == StructureState.Idle)
                            {
                                volTarget = curMusicStructure.volume;
                            }
                            else
                            {
                                volTarget = curMusicStructure.volume * .5f;
                                //if (structureStateIndex >= structureStateIndexMax)
                                //{
                                //    volTarget = 0f;
                                //}
                            }
                        }
                    }
                }

                //Debug.Log("volTarget: " + volTarget.ToString() + " || " + Time.time.ToString());

                if ( SetupManager.instance.hideMusic )
                {
                    volTarget = 0f;
                    for (int i = 0; i < musicAudioSources.Length; i++)
                    {
                        musicAudioSources[i].volume = 0f;
                    }
                }

                float nearFountainFac = (SetupManager.instance.playerInFountainArea) ? 0f : 1f;
                float nearFountainVolLerpie = (SetupManager.instance.playerInFountainArea) ? 3f : 1f;
                float volLerpie = (GameManager.instance.playerInCombat) ? 5f : .25f;
                for (int i = 0; i < musicAudioSources.Length; i++)
                {
                    musicAudioSources[i].volume = Mathf.Lerp(musicAudioSources[i].volume, (volTarget * SetupManager.instance.musicVolFactor) * nearFountainFac, (volLerpie * nearFountainVolLerpie) * Time.deltaTime);
                }

                // next segment
                if ((time + 1f) > nextEventTime)
                {
                    structureStateIndex++;
                    if (structureStateIndex > structureStateIndexMax)
                    {
                        structureStateIndex = 0;
                        SetStructureState(nextStructureState);
                    }

                    PlayNextStructure();
                }
            }
        }
        else
        {
            StopMusicStructure();
        }
    }

    void PlayNextStructure ()
    {
        AudioClip clipToPlay = null;
        switch (curStructureState)
        {
            case StructureState.Idle: clipToPlay = curMusicStructure.clipIdle[structureStateIndex]; break;
            case StructureState.Low: clipToPlay = curMusicStructure.clipLow[structureStateIndex]; break;
            case StructureState.Medium: clipToPlay = curMusicStructure.clipMedium[structureStateIndex]; break;
            case StructureState.High: clipToPlay = curMusicStructure.clipHigh[structureStateIndex]; break;
        }

        float scheduleOff = 0f;//.5f;

        musicAudioSources[flip].clip = clipToPlay;
        musicAudioSources[flip].PlayScheduled(nextEventTime - scheduleOff);

        float timeAdd = (60f / curMusicStructure.bpm * curMusicStructure.numBeatsPerSegment);
        nextEventTime += timeAdd;

        flip = 1 - flip;

        float volumeTimeAdd = (timeAdd + 1f);
        Invoke("UpdateVolumeStructureState",volumeTimeAdd);

        // log
        //Debug.Log("Scheduled source " + flip + " to start at time " + nextEventTime + " || clipToPlay: " + clipToPlay + " || " + Time.time.ToString());
    }

    void UpdateVolumeStructureState ()
    {
        volumeStructureStateRead = curStructureState;

        // log
        //Debug.Log("update volume structure || " + Time.time.ToString());
    }

    void SetStructureState(StructureState _to)
    {
        if (curStructureState != _to)
        {
            curStructureState = _to;
        }
    }

    public void SetMusicStructure(MusicStructure _to)
    {
        curMusicStructure = _to;
    }

    public void SetMusicTarget(MusicTarget _to)
    {
        curMusicTarget = _to;
    }

    [System.Serializable]
    public struct MusicStructure
    {
        public float volume;
        public float bpm;
        public int numBeatsPerSegment;
        public AudioClip[] clipIdle;
        public AudioClip[] clipLow;
        public AudioClip[] clipMedium;
        public AudioClip[] clipHigh;
    }
}