using UnityEngine;

namespace Npc
{
    [System.Serializable]
    public enum Type
    {
        Rat,
        Bat,
        Goblin,
        Slug,
        SmallSpider,
        Ghost,
        RatWarrior,
        FireMage,
        RatKing,
        SmallRat,
        MagicSkull,
        Faerie,
        HellLord,
        HellPopje,
        HellFlyingFace,
        Skeleton,
        RedSkeleton,
        BlackSkeleton,
        FireDevil,
        FireBat,
        SmallFireRat,
        OrcWarrior,
        OrcMage,
        MediumSpider,
        BigSpider,
        BigSlug,
        BigRat,
        GoblinRanger,
        GoblinMage,
        GoblinChieftain,
        SmallRockGolem,
        OrcLeader,
        Length
    }

    [System.Serializable]
    public struct AttackData
    {
        public enum AttackType { Melee, Ranged, MinionSpawn, WakeBoss, HealSelf, HealRadius };
        public AttackType attackType;

        public enum DamageType { Melee, Magic, Special };
        public DamageType damageType;

        public enum RangedShootFromType { Default, Eye, Mouth };
        public RangedShootFromType rangedShootFromType;

        public enum RangedAttackType { Single, Spray, Laser };
        public RangedAttackType rangedAttackType;
        public int sprayCount;
        public int sprayInterval;

        public int healAmount;
        public int healRadius;

        public float rangedSpeedFactor;
        public float rangedGravityFactor;

        public Vector2 rangedSideOffMax;
        public Vector2 rangedFwdOffMax;
        public Vector2 rangedUpOffMax;

        public bool leaveFireTrail;
        public GameObject fireTrailProjectilePrefab;
        public int fireTrailRate;
        public Vector3 fireTrailDirOffMin;
        public Vector3 fireTrailDirOffMax;

        public int damage;
        public float fwdForceFac;

        public float stopChaseDstAdd;
        public GameObject projectilePrefab;

        public float damageDealExtraRadius;
        public Vector3 damageDealSpawnLocalAdd;

        public int attackPrepareDur;
        public int attackDoDur;

        public MinionSpawnData[] minionSpawnDatas;

        public bool facePlayer;

        public bool openMouthWhenPreparing;

        public float weight;
    }

    [System.Serializable]
    public struct MinionSpawnData
    {
        public Npc.Type type;
        public int weight;
    }

    [System.Serializable]
    public struct Info
    {
        public GameObject prefab;
        public Stats stats;
        public Graphics graphics;
        public Audio audio;
        public AttackData[] attacks;
    }

    [System.Serializable]
    public struct Stats
    {
        public string name;
        public int strength;
        public int health;
        public bool healthIsFire;
        public bool healFromFire;
        public float movementSpd;
        public float chaseStopDst;
        public int tearDropCount;
        public float hitForceFactor;
        public float randomBlockChance;
        public float reactBlockChance;
        public bool canBlockProjectiles;
        public bool canRedirectProjectiles;
        public int hitDur;
        public bool meleeImmune;
        public bool magicImmune;
        public bool immortal;
        public bool fleeFromPlayer;
        public Vector3 hitBoxCenterOff;
        public Vector3 hitBoxScaleFac;
        public float hitBoxExtraRadius;
        public int vulnerableDur;
        public bool onlyRaakbaarWhenVulnerable;
        public Vector3 defeatWhiteOrbOffset;
        public float defeatWhiteOrbScaleFactor;
        public float bounceIntoPlayerCheckDst;
        public bool hideOnStart;
        public float alertDstExtra;
    }

    [System.Serializable]
    public struct Graphics
    {
        public bool flying;
        public float flyHeight;
        public float flyHeightEnterNextStage;
        public float flyTimeFactor;
        public float flyDstFactor;
        public float flyLegOff;

        public bool hasWings;
        public float wingScl;
        public Mesh wingMesh;
        public float wingTimeFactor;
        public float wingDstFactor;
        public float wingSideDst;
        public float wingUpDst;
        public float wingFwdDst;
        public Vector3 wingRotOff;

        public bool hasBody;
        public Vector3 bodyOffset;
        public float bodyHitFwdRot;
        public float bodyAlertedFwdRot;
        public float bodyAttackPrepareFwdRot;
        public float bodyAttackDoFwdRot;
        public float bodyBlockFwdRot;

        public bool bodySlugAnimation;
        public float bodySlugTimeFactor;
        public float bodySlugDstFactor;

        public bool hasHead;
        public Vector3 headOffset;
        public float headAnimTimeFactor;
        public float headAnimDstFactor;

        public bool eyeHasFireCharge;
        public GameObject eyeFireChargePrefab;
        public float eyeFireChargeScaleFactor;

        public bool hasLegs;
        public int legCount;
        public float legWidth;
        public float legLength;
        public float legSpaceDistance;
        public float legSpreadFactor;
        public float legRowEndSpreadFactor;
        public float hipWidth;

        public float kneeFwdOff;
        public float kneeSideOff;
        public float kneeUpOff;

        public int stepRate;
        public float stepFwdFactor;
        public float stepUpFactor;

        public bool hasFeet;
        public float feetScale;
        public Mesh feetMesh;

        public bool hasArms;
        public int armCount;
        public float armWidth;
        public float armLength;
        public float armSpreadFactor;
        public float armUpFac;

        public float elbowFwdOff;
        public float elbowSideOff;

        public float shoulderWidth;
        public float shoulderHeight;

        public bool hasHand;
        public float handScale;
        public Mesh handMesh;

        public bool hasTail;
        public float tailWidth;
        public float tailWidthTipFac;
        public float tailLength;
        public Vector3 tailOffset;
        public float tailVerAdd;

        public bool hasMouth;
        public float mouthAttackSpreadFac;
        public float mouthAttackPrepareSpreadExtra;
        public float mouthHitSpreadFac;
        public float mouthBlockSpreadFac;

        public float alertedJumpHeight;
        public float alertedLegSpread;
        public float alertedArmSpread;

        public float attackPrepareHeightOff;
        public float attackPrepareLegSpread;
        public float attackPrepareArmSpread;

        public float attackDoHeightOff;
        public float attackDoLegSpread;
        public float attackDoArmSpread;

        public float attackDoLegFwdOff;
        public float attackDoLegAnimationFwdOff;
        public float attackDoLegAnimationUpOff;
        public float attackDoLegAnimationSideOff;

        public float attackDoArmFwdOff;
        public float attackDoArmAnimationFwdOff;
        public float attackDoArmAnimationUpOff;
        public float attackDoArmAnimationSideOff;

        public Vector3 attackPrepareShakeTime;
        public Vector3 attackPrepareShakeDst;

        public bool mouthHasFireCharge;
        public GameObject mouthFireChargePrefab;
        public float mouthFireChargeScaleFactor;
        public float mouthFireChargeGrowTarget;

        public float hitLegTimeFactor;
        public float hitLegDstFactor;

        public float hitArmTimeFactor;
        public float hitArmDstFactor;

        public float healthIndicatorOff;
        public float healthIndicatorExtraRange;

        public Vector3 damageIndicatorLocalAdd;

        public bool hasEquipment;
        public EquipmentDatabase.Equipment[] equipments;
        public Vector3[] equipmentHoldOffsets;

        public Material legMat;
        public Material armMat;
        public Material tailMat;
        public Material feetMat;
        public Material handMat;
        public Material wingMat;
    }

    [System.Serializable]
    public struct Audio
    {
        public float distanceAdd;

        public AudioClip[] alertClips;
        public float alertPitchMin, alertPitchMax;
        public float alertVolumeMin, alertVolumeMax;

        public AudioClip[] attackPrepareClips;
        public float attackPreparePitchMin, attackPreparePitchMax;
        public float attackPrepareVolumeMin, attackPrepareVolumeMax;

        public AudioClip[] attackDoClips;
        public float attackDoPitchMin, attackDoPitchMax;
        public float attackDoVolumeMin, attackDoVolumeMax;

        public AudioClip[] hurtClips;
        public float hurtPitchMin, hurtPitchMax;
        public float hurtVolumeMin, hurtVolumeMax;

        public AudioClip[] deadClips;
        public float deadPitchMin, deadPitchMax;
        public float deadVolumeMin, deadVolumeMax;
    }
}
