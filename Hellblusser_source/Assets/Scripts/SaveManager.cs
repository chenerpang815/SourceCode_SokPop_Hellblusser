using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    // save path
    string savePath;

    void Awake ()
    {
        instance = this;

        // save path
        savePath = Path.Combine(Application.persistentDataPath, "hellblusserSavefile.txt");

        // clear?
        if (Application.isEditor)
        {
            //ClearFile();
        }
    }

    public void StoreLastLevelData ( ref SetupManager.RunData _dataTo )
    {
        _dataTo.playerHealthOnStartOfLevel = _dataTo.playerHealthCur;
        _dataTo.playerFireOnStartOfLevel = _dataTo.playerFireCur;
        _dataTo.coinsOnStartOfLevel = _dataTo.curRunCoinsCollected;
        _dataTo.tearsOnStartOfLevel = _dataTo.curRunTearsCollected;
        _dataTo.usedSecondChanceOnStartOfLevel = _dataTo.usedSecondChance;
        _dataTo.runTimeOnStartOfLevel = _dataTo.runTime;

        _dataTo.ringEquipmentIndexOnStartOfLevel = _dataTo.curRingEquipmentIndex;
        _dataTo.braceletEquipmentIndexOnStartOfLevel = _dataTo.curBraceletEquipmentIndex;
        _dataTo.bodyEquipmentIndexOnStartOfLevel = _dataTo.curBodyEquipmentIndex;
        _dataTo.weaponEquipmentIndexOnStartOfLevel = _dataTo.curWeaponEquipmentIndex;

        _dataTo.blessingsClaimedOnStartOflevel = new List<int>();
        if (_dataTo.blessingsClaimed != null)
        {
            for (int i = 0; i < _dataTo.blessingsClaimed.Count; i++)
            {
                _dataTo.blessingsClaimedOnStartOflevel.Add(_dataTo.blessingsClaimed[i]);
            }
        }
    }

    public void GetLastLevelData ( ref SetupManager.RunData _dataFrom )
    {
        _dataFrom.playerHealthCur = _dataFrom.playerHealthOnStartOfLevel;
        _dataFrom.playerFireCur = _dataFrom.playerFireOnStartOfLevel;
        _dataFrom.curRunCoinsCollected = _dataFrom.coinsOnStartOfLevel;
        _dataFrom.curRunTearsCollected = _dataFrom.tearsOnStartOfLevel;
        _dataFrom.usedSecondChance = _dataFrom.usedSecondChanceOnStartOfLevel;
        _dataFrom.runTime = _dataFrom.runTimeOnStartOfLevel;

        _dataFrom.curRingEquipmentIndex = _dataFrom.ringEquipmentIndexOnStartOfLevel;
        _dataFrom.curBraceletEquipmentIndex = _dataFrom.braceletEquipmentIndexOnStartOfLevel;
        _dataFrom.curBodyEquipmentIndex = _dataFrom.bodyEquipmentIndexOnStartOfLevel;
        _dataFrom.curWeaponEquipmentIndex = _dataFrom.weaponEquipmentIndexOnStartOfLevel;

        _dataFrom.blessingsClaimed = new List<int>();
        if (_dataFrom.blessingsClaimedOnStartOflevel != null)
        {
            for (int i = 0; i < _dataFrom.blessingsClaimedOnStartOflevel.Count; i++)
            {
                _dataFrom.blessingsClaimed.Add(_dataFrom.blessingsClaimedOnStartOflevel[i]);
            }
        }

        // log
        //Debug.LogError("get last level data || weapon index: " +  + Time.time.ToString());
    }

    public void WriteToFile(SetupManager.ProgressData _newData)
    {
        if (SetupManager.instance == null || (SetupManager.instance != null && !SetupManager.instance.aboutToDeleteSave))
        {
            bool fileExists = CheckIfFileExists();

            string json = JsonUtility.ToJson(_newData);

            File.WriteAllText(savePath, json);

            //Debug.LogError("writing to file, file exists: " + fileExists + " || " + Time.time.ToString());
        }
    }

    public SetupManager.ProgressData LoadFromFile ()
    {
        SetupManager.ProgressData saveDataReturn = new SetupManager.ProgressData();
        bool fileExists = CheckIfFileExists();
        if (fileExists)
        {
            string json = File.ReadAllText(savePath);

            saveDataReturn = JsonUtility.FromJson<SetupManager.ProgressData>(json);

            // create new run because we died last turn?
            if (saveDataReturn.normalRunData.playerDead)
            {
                saveDataReturn.normalRunData = CreateNewRunData();
                saveDataReturn.normalRunData.runSeed = 0;

                saveDataReturn.normalRunData.curBodyEquipmentIndex = (int)EquipmentDatabase.Equipment.BlueCloak;
                saveDataReturn.normalRunData.curWeaponEquipmentIndex = (int)EquipmentDatabase.Equipment.IronSword;

                StoreLastLevelData(ref saveDataReturn.normalRunData);
            }
            if (saveDataReturn.normalRunData.curLevelIndex >= (saveDataReturn.normalRunData.curFloorData.locationCount))
            {
                saveDataReturn.normalRunData.curFloorIndex++;
                saveDataReturn.normalRunData.curLevelIndex = 0;
                saveDataReturn.normalRunData.encountersHad = 0;
            }

            if (saveDataReturn.endlessRunData.playerDead)
            {
                saveDataReturn.endlessRunData = CreateNewRunData();
                saveDataReturn.endlessRunData.runSeed = Mathf.RoundToInt(TommieRandom.instance.RandomRange(-1000000f, 1000000f));

                // start endless run with white cloak
                saveDataReturn.endlessRunData.curBodyEquipmentIndex = (int)EquipmentDatabase.Equipment.WhiteCloak;

                StoreLastLevelData(ref saveDataReturn.endlessRunData);
            }
            if (saveDataReturn.endlessRunData.curLevelIndex >= (saveDataReturn.endlessRunData.curFloorData.locationCount))
            {
                saveDataReturn.endlessRunData.curFloorIndex++;
                saveDataReturn.endlessRunData.curLevelIndex = 0;
                saveDataReturn.endlessRunData.encountersHad = 0;
            }

        }
        return saveDataReturn;
    }

    public void ClearFile()
    {
        bool fileExists = CheckIfFileExists();
        if (fileExists)
        {
            File.Delete(savePath);
        }

        // log
        //Debug.LogError("wants to clear file || fileExists: " + fileExists + " || " + Time.time.ToString());
    }

    public bool CheckIfFileExists ()
    {
        return File.Exists(savePath);
    }

    public SetupManager.ProgressData CreateNewData ()
    {
        Debug.LogError("create new global data || " + Time.time.ToString());

        SetupManager.ProgressData newData = new SetupManager.ProgressData();

        SetupManager.PersistentData newPersistentData = new SetupManager.PersistentData();
        newPersistentData.sawIntro = false;
        newPersistentData.sawOutro = false;
        newPersistentData.unlockedEndless = false;
        newPersistentData.inNormalRun = false;
        newPersistentData.inEndlessRun = false;
        newPersistentData.playTime = 0f;

        newPersistentData.sawCombatPopup = false;
        newPersistentData.sawFirePopup = false;
        newPersistentData.sawFireCollectPopup = false;
        newPersistentData.sawKickPopup = false;
        newPersistentData.sawTearPopup = false;
        newPersistentData.sawDonutPopup = false;
        newPersistentData.sawCoinPopup = false;
        newPersistentData.sawShopPopup = false;
        newPersistentData.sawFountainPopup = false;
        newPersistentData.sawKeyPopup = false;
        newPersistentData.sawRestPopup = false;
        newPersistentData.sawEndlessPopup = false;
        newPersistentData.sawMaxFirePopup = false;

        newPersistentData.normalHighestLoopIndex = 0;
        newPersistentData.normalHighestFloorIndex = 0;
        newPersistentData.normalHighestLevelIndex = 0;

        newPersistentData.endlessHighestLoopIndex = 0;
        newPersistentData.endlessHighestFloorIndex = 0;
        newPersistentData.endlessHighestLevelIndex = 0;

        newData.persistentData = newPersistentData;

        newData.normalRunData = CreateNewRunData();
        newData.normalRunData.runSeed = 0;

        newData.endlessRunData = CreateNewRunData();
        newData.endlessRunData.runSeed = Mathf.RoundToInt(TommieRandom.instance.RandomRange(-1000000f,1000000f));

        // hack
        newData.endlessRunData.curBodyEquipmentIndex = (int)EquipmentDatabase.Equipment.WhiteCloak;
        //newData.endlessRunData.curBraceletEquipmentIndex = (int)EquipmentDatabase.Equipment.SilverBracelet;
        //newData.endlessRunData.curRingEquipmentIndex = (int)EquipmentDatabase.Equipment.SilverRing;
        //newData.endlessRunData.curWeaponEquipmentIndex = (int)EquipmentDatabase.Equipment.BasicWand;

        // settings
        SetupManager.SettingsData newSettingsData = new SetupManager.SettingsData();
       
        int maxResolutionIndex = (Screen.resolutions.Length - 1);

        newSettingsData.resolutionIndex = maxResolutionIndex;
        newSettingsData.fullscreen = 1;

        newSettingsData.sfxVolIndex = 0;
        newSettingsData.musicVolIndex = 0;

        newSettingsData.lookSensitivityX = 0;
        newSettingsData.lookSensitivityY = 0;
        newSettingsData.invertY = 0;

        newSettingsData.wobbleEffect = 1;
        newSettingsData.cameraMotion = 1;
        newSettingsData.endlessFilter = 1;

        newData.settingsData = newSettingsData;

        return newData;
    }

    public SetupManager.RunData CreateNewRunData ()
    {
        Debug.LogError("create new run || " + Time.time.ToString());

        SetupManager.RunData newRunData = new SetupManager.RunData();

        newRunData.runTime = 0f;
        newRunData.runSeed = 0;
        newRunData.playerReachedEnd = false;
        newRunData.playerHealthMax = (SetupManager.instance.playerStrong) ? 99 : 6;
        newRunData.playerHealthCur = newRunData.playerHealthMax;
        newRunData.playerFireMax = 3;
        newRunData.playerFireCur = 0;
        newRunData.curLoopIndex = 0;
        newRunData.curFloorIndex = 1;
        newRunData.curLevelIndex = 0;
        newRunData.encountersHad = 0;
        newRunData.blessingsClaimed = new List<int>();

        /*
        if (Application.isEditor)
        {
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.SwiftStrikes);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.QuickFeet);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.GlassCannon);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.FireBurst);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Warrior);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Sorcerer);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Tank);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.SecondChance);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Lucky);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Mournful);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.WildFire);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.AgileJumper);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Acrobat);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Spray);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.FireRage);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Revenge);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Hungry);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Resilient);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Thrifty);
            newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.Drainer);
            //newRunData.blessingsClaimed.Add((int)BlessingDatabase.Blessing.HotFire);
        }
        */

        newRunData.playerDead = false;
        newRunData.usedSecondChance = false;

        newRunData.curBodyEquipmentIndex = (int)(EquipmentDatabase.Equipment.BlueCloak);
        newRunData.curRingEquipmentIndex = -1;//(int)(EquipmentDatabase.Equipment.GoldRing);//-1;
        newRunData.curBraceletEquipmentIndex = -1;//(int)(EquipmentDatabase.Equipment.SilverBracelet);//-1;
        newRunData.curWeaponEquipmentIndex = (int)(EquipmentDatabase.Equipment.IronSword);

        // generate new floor data
        SetupManager.FloorData newFloorData = new SetupManager.FloorData();
        int locationCount = 8;
        int levelsBeforeShop = 0;
        newFloorData.locationCount = locationCount;
        newFloorData.locationTypes = new List<SetupManager.LocationData>();
        //newFloorData.locationTypes = new List<List<int>>();
        newFloorData.locationVisitedIndex = new List<int>();
        for (int i = 0; i < locationCount; i++)
        {
            SetupManager.LocationData newLocationData = new SetupManager.LocationData();

            newFloorData.locationVisitedIndex.Add(-1);
            List<int> curLocationTypes = new List<int>();
            if (levelsBeforeShop < 2)
            {
                curLocationTypes.Add((i == (locationCount - 1)) ? (int)SetupManager.LocationType.BossLevel : (int)SetupManager.LocationType.Level);
                levelsBeforeShop++;
            }
            else
            {
                curLocationTypes.Add((int)SetupManager.LocationType.Shop);
                curLocationTypes.Add((int)SetupManager.LocationType.Rest);
                levelsBeforeShop = 0;
            }
            newLocationData.types = curLocationTypes;
            newFloorData.locationTypes.Add(newLocationData);
        }
        newRunData.curFloorData = newFloorData;

        return newRunData;
    }
}
