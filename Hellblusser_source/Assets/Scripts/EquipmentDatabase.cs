using UnityEngine;
using System.Collections.Generic;

public class EquipmentDatabase : MonoBehaviour
{
    // instance
    public static EquipmentDatabase instance;

    // visuals
    [Header("visuals")]
    public GameObject copperRingVisualsObject;
    public GameObject silverRingVisualsObject;
    public GameObject goldRingVisualsObject;
    public GameObject copperBraceletVisualsObject;
    public GameObject silverBraceletVisualsObject;
    public GameObject goldBraceletVisualsObject;
    public GameObject blueCloakVisualsObject;
    public GameObject redCloakVisualsObject;
    public GameObject greenCloakVisualsObject;
    public GameObject whiteCloakVisualsObject;
    public GameObject ironSwordVisualsObject;
    public GameObject silverSwordVisualsObject;
    public GameObject bronzeSwordVisualsObject;
    public GameObject woodenClubVisualsObject;
    public GameObject basicWandVisualsObject;
    public GameObject torchVisualsObject;
    public GameObject simpleMaceVisualsObject;
    public GameObject steelDaggerVisualsObject;
    public GameObject drainBladeVisualsObject;
    public GameObject fireBladeVisualsObject;
    public GameObject chargeBladeVisualsObject;
    public GameObject cloakOfSadnessVisualsObject;
    public GameObject sturdyCloakVisualsObject;
    public GameObject sneakyCloakVisualsObject;
    public GameObject goldBladeVisualsObject;

    // position
    [HideInInspector] public float ringShopSclFactor;
    [HideInInspector] public Vector3 ringShopPosOff;
    [HideInInspector] public Vector3 ringShopRotOff;

    [HideInInspector] public float braceletShopSclFactor;
    [HideInInspector] public Vector3 braceletShopPosOff;
    [HideInInspector] public Vector3 braceletShopRotOff;

    [HideInInspector] public float weaponShopSclFactor;
    [HideInInspector] public Vector3 weaponShopPosOff;
    [HideInInspector] public Vector3 weaponShopRotOff;

    [HideInInspector] public float bodyShopSclFactor;
    [HideInInspector] public Vector3 bodyShopPosOff;
    [HideInInspector] public Vector3 bodyShopRotOff;

    // sprites
    [Header("sprites")]
    public Sprite equipmentRingSprite;
    public Sprite equipmentBraceletSprite;
    public Sprite equipmentWeaponSprite;
    public Sprite equipmentBodySprite;
    public Sprite equipmentEmptySprite;

    // materials
    [Header("materials")]
    public Material blueCloakMatA;
    public Material blueCloakMatB;
    public Material blueCloakMatC;
    public Material redCloakMatA;
    public Material redCloakMatB;
    public Material redCloakMatC;
    public Material greenCloakMatA;
    public Material greenCloakMatB;
    public Material greenCloakMatC;
    public Material whiteCloakMatA;
    public Material whiteCloakMatB;
    public Material whiteCloakMatC;
    public Material sadnessCloakMatA;
    public Material sadnessCloakMatB;
    public Material sadnessCloakMatC;
    public Material sturdyCloakMatA;
    public Material sturdyCloakMatB;
    public Material sturdyCloakMatC;
    public Material sneakyCloakMatA;
    public Material sneakyCloakMatB;
    public Material sneakyCloakMatC;

    // items
    public enum Equipment
    {
        //SilverRing,
        //GoldRing,
        //CopperBracelet,
        //SilverBracelet,
        //GoldBracelet,
        BlueCloak,
        RedCloak,
        //GreenCloak,
        WhiteCloak,
        IronSword,
        //BronzeSword,
        //SilverSword,
        WoodenClub,
        BasicWand,
        Torch,
        RingOfHaste,
        SimpleMace,
        ExplosiveRing,
        JumpExplodeBracelet,
        RecoilBracelet,
        SteelDagger,
        DrainBlade,
        FireBlade,
        KickRing,
        ChargeBlade,
        CloakOfSadness,
        SturdyCloak,
        SneakyCloak,
        KnockbackBracelet,
        GoldBlade,
    };

    // specials
    public enum Specials
    {
        EnemiesExplodeOnDefeat,
        CantStopWalking,
        ExplosionWhenJumping,
        Recoil,
        CantBlock,
        DrainHealth,
        ShootFire,
        KickDamage,
        FireCharge,
        EnemiesDropExtraTear,
        IgnoreDamage,
        EnemiesDropExtraCoin,
        PlayerShedTear,
    };

    // type
    public enum Slot
    {
        Body,
        Ring,
        Bracelet,
        Weapon
    };

    // dictionary
    public Dictionary<Equipment,EquipmentData> equipmentDatas;

    void Awake ()
    {
        instance = this;

        // init
        Init();
    }

    void Init ()
    {
        // shop offsets
        ringShopPosOff = new Vector3(0f, .0675f, 0f);
        ringShopRotOff = Vector3.zero;
        ringShopSclFactor = 1f;

        braceletShopPosOff = new Vector3(0f, .1f, 0f);
        braceletShopRotOff = Vector3.zero;
        braceletShopSclFactor = 1f;

        weaponShopPosOff = new Vector3(0f, .1f, 0f);
        weaponShopRotOff = new Vector3(90f, 45f, 0f);
        weaponShopSclFactor = .175f;

        bodyShopPosOff = new Vector3(0f, .1f, 0f);
        bodyShopRotOff = Vector3.zero;
        bodyShopSclFactor = 1f;

        // create dictionary
        equipmentDatas = new Dictionary<Equipment, EquipmentData>();

        EquipmentStats explosiveRingStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> explosiveRingSpecialPackages = new List<SpecialsPackage> { new SpecialsPackage(Specials.EnemiesExplodeOnDefeat,100) };
        equipmentDatas.Add(Equipment.ExplosiveRing, new EquipmentData { name = "explosive ring", nameFormatted = "explosive" + "\n" + "ring", description = "enemies explode when defeated", slot = Slot.Ring, visualsObject = copperRingVisualsObject, shopCoinCost = 10, stats = explosiveRingStats, specialsPackage = explosiveRingSpecialPackages });

        EquipmentStats ringOfHasteStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> ringOfHasteSpecialsPackages = new List<SpecialsPackage>();
        equipmentDatas.Add(Equipment.RingOfHaste, new EquipmentData { name = "ring of haste", nameFormatted = "ring" + "\n" + "of haste", description = "faster movement speed", slot = Slot.Ring, visualsObject = silverRingVisualsObject, shopCoinCost = 10, stats = ringOfHasteStats, specialsPackage = ringOfHasteSpecialsPackages });

        EquipmentStats simpleMaceStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = .25f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 1, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> simpleMaceSpecialsPackages = new List<SpecialsPackage>();
        equipmentDatas.Add(Equipment.SimpleMace, new EquipmentData { name = "simple mace", nameFormatted = "simple" + "\n" + "mace", description = "lower attack speed" + "\n" + "+1 melee damage", slot = Slot.Weapon, visualsObject = simpleMaceVisualsObject, shopCoinCost = 5, stats = simpleMaceStats, specialsPackage = simpleMaceSpecialsPackages });

        EquipmentStats jumpExplodeBraceletStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> jumpExplodeBraceletSpecialsPackages = new List<SpecialsPackage> { new SpecialsPackage(Specials.ExplosionWhenJumping,100) };
        equipmentDatas.Add(Equipment.JumpExplodeBracelet, new EquipmentData { name = "wild bracelet", nameFormatted = "wild" + "\n" + "bracelet", description = "create an explosion when jumping", slot = Slot.Bracelet, visualsObject = copperBraceletVisualsObject, shopCoinCost = 10, stats = jumpExplodeBraceletStats, specialsPackage = jumpExplodeBraceletSpecialsPackages });

        EquipmentStats recoilBraceletStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> recoilBraceletSpecialsPackages = new List<SpecialsPackage> { new SpecialsPackage(Specials.Recoil,100) };
        equipmentDatas.Add(Equipment.RecoilBracelet, new EquipmentData { name = "bracelet of recoil", nameFormatted = "bracelet of" + "\n" + "recoil", description = "when damaged, deal 1 damage to" + "\n" + "the attacker", slot = Slot.Bracelet, visualsObject = silverBraceletVisualsObject, shopCoinCost = 10, stats = recoilBraceletStats, specialsPackage = recoilBraceletSpecialsPackages });

        EquipmentStats redCloakStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 2, maxHealthAdd = 0 };
        List<SpecialsPackage> redCloakSpecialsPackages = new List<SpecialsPackage> {};
        equipmentDatas.Add(Equipment.RedCloak, new EquipmentData { name = "warm cloak", nameFormatted = "warm" + "\n" + "cloak", description = "+2 fire capacity", slot = Slot.Body, visualsObject = redCloakVisualsObject, shopCoinCost = 15, matA = redCloakMatA, matB = redCloakMatB, matC = redCloakMatC, stats = redCloakStats, specialsPackage = redCloakSpecialsPackages });

        EquipmentStats steelDaggerStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = -.25f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> steelDaggerSpecialsPackages = new List<SpecialsPackage> { new SpecialsPackage(Specials.CantBlock,100) };
        equipmentDatas.Add(Equipment.SteelDagger, new EquipmentData { name = "steel dagger", nameFormatted = "steel" + "\n" + "dagger", description = "higher attack speed" + "\n" + "but can't block", slot = Slot.Weapon, visualsObject = steelDaggerVisualsObject, shopCoinCost = 10, stats = steelDaggerStats, specialsPackage = steelDaggerSpecialsPackages });

        EquipmentStats drainBladeStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = -.05f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> drainBladeSpecialsPackages = new List<SpecialsPackage> { new SpecialsPackage(Specials.DrainHealth,10) };
        equipmentDatas.Add(Equipment.DrainBlade, new EquipmentData { name = "drain blade", nameFormatted = "drain" + "\n" + "blade", description = "10 percent chance to heal 1" + "\n" + "when defeating enemies", slot = Slot.Weapon, visualsObject = drainBladeVisualsObject, shopCoinCost = 20, stats = drainBladeStats, specialsPackage = drainBladeSpecialsPackages });

        EquipmentStats fireBladeStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = .25f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> fireBladeSpecialsPackages = new List<SpecialsPackage> { new SpecialsPackage(Specials.ShootFire, 100) };
        equipmentDatas.Add(Equipment.FireBlade, new EquipmentData { name = "fire blade", nameFormatted = "fire" + "\n" + "blade", description = "shoots fire when attacking" + "\n" + "lower attack speed", slot = Slot.Weapon, visualsObject = fireBladeVisualsObject, shopCoinCost = 50, stats = fireBladeStats, specialsPackage = fireBladeSpecialsPackages });

        equipmentDatas.Add(Equipment.IronSword, new EquipmentData { name = "iron sword", nameFormatted = "iron" + "\n" + "sword", description = "a basic sword", slot = Slot.Weapon, visualsObject = ironSwordVisualsObject, shopCoinCost = 5 });

        EquipmentStats kickRingStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 1, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> kickRingSpecialsPackages = new List<SpecialsPackage> { new SpecialsPackage(Specials.KickDamage,100f) };
        equipmentDatas.Add(Equipment.KickRing, new EquipmentData { name = "ring of kicking", nameFormatted = "ring" + "\n" + "of kicking", description = "kicks deal melee damage" + "\n" + "but also have knockback", slot = Slot.Ring, visualsObject = silverRingVisualsObject, shopCoinCost = 15, stats = kickRingStats, specialsPackage = kickRingSpecialsPackages });

        EquipmentStats chargeBladeStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> chargeBladeSpecialsPackages = new List<SpecialsPackage> { new SpecialsPackage(Specials.FireCharge, 20) };
        equipmentDatas.Add(Equipment.ChargeBlade, new EquipmentData { name = "charge blade", nameFormatted = "charge" + "\n" + "blade", description = "20 percent chance to gain" + "\n" + "1 fire when defeating enemies", slot = Slot.Weapon, visualsObject = chargeBladeVisualsObject, shopCoinCost = 25, stats = chargeBladeStats, specialsPackage = chargeBladeSpecialsPackages });

        EquipmentStats cloakOfSadnessStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> cloakOfSadnessSpecialsPackage = new List<SpecialsPackage> { new SpecialsPackage(Specials.PlayerShedTear, 100) };
        equipmentDatas.Add(Equipment.CloakOfSadness, new EquipmentData { name = "cloak of sadness", nameFormatted = "cloak of" + "\n" + "sadness", description = "shed a tear when damaged", slot = Slot.Body, visualsObject = cloakOfSadnessVisualsObject, shopCoinCost = 10, stats = cloakOfSadnessStats, specialsPackage = cloakOfSadnessSpecialsPackage, matA = sadnessCloakMatA, matB = sadnessCloakMatB, matC = sadnessCloakMatC });

        EquipmentStats sturdyCloakStats = new EquipmentStats { moveSpeedAdd = -.5f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> sturdyCloakSpecialsPackage = new List<SpecialsPackage> { new SpecialsPackage(Specials.IgnoreDamage, 20) };
        equipmentDatas.Add(Equipment.SturdyCloak, new EquipmentData { name = "sturdy cloak", nameFormatted = "sturdy" + "\n" + "cloak", description = "20 percent chance to ignore damage" + "\n" + "but lower movement speed", slot = Slot.Body, visualsObject = sturdyCloakVisualsObject, shopCoinCost = 20, stats = sturdyCloakStats, specialsPackage = sturdyCloakSpecialsPackage, matA = sturdyCloakMatA, matB = sturdyCloakMatB, matC = sturdyCloakMatC });

        EquipmentStats sneakyCloakStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = -2f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> sneakyCloakSpecialsPackage = new List<SpecialsPackage> {};
        equipmentDatas.Add(Equipment.SneakyCloak, new EquipmentData { name = "sneaky cloak", nameFormatted = "sneaky" + "\n" + "cloak", description = "enemies have slightly more trouble" + "\n" + "detecting you", slot = Slot.Body, visualsObject = sneakyCloakVisualsObject, shopCoinCost = 10, stats = sneakyCloakStats, specialsPackage = sneakyCloakSpecialsPackage, matA = sneakyCloakMatA, matB = sneakyCloakMatB, matC = sneakyCloakMatC });

        EquipmentStats knockbackBraceletStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 10f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 1 };
        List<SpecialsPackage> knockbackBraceletSpecialsPackage = new List<SpecialsPackage> { };
        equipmentDatas.Add(Equipment.KnockbackBracelet, new EquipmentData { name = "knockback bracelet", nameFormatted = "knockback" + "\n" + "bracelet", description = "+1 max health" + "\n" + "but more knockback", slot = Slot.Bracelet, visualsObject = silverBraceletVisualsObject, shopCoinCost = 15, stats = knockbackBraceletStats, specialsPackage = knockbackBraceletSpecialsPackage });

        EquipmentStats goldBladeStats = new EquipmentStats { moveSpeedAdd = 0f, attackSpeedAdd = 0f, knockbackAdd = 0f, alertDstAdd = 0f, meleeDamageAdd = 0, magicDamageAdd = 0, maxFireAdd = 0, maxHealthAdd = 0 };
        List<SpecialsPackage> goldBladeSpecialsPackage = new List<SpecialsPackage> { new SpecialsPackage(Specials.EnemiesDropExtraCoin, 20) };
        equipmentDatas.Add(Equipment.GoldBlade, new EquipmentData { name = "gold blade", nameFormatted = "gold" + "\n" + "blade", description = "enemies have a 20 percent chance" + "\n" + "to drop a coin when defeated", slot = Slot.Weapon, visualsObject = goldBladeVisualsObject, shopCoinCost = 30, stats = goldBladeStats, specialsPackage = goldBladeSpecialsPackage });

        //equipmentDatas.Add(Equipment.SilverRing, new EquipmentData { name = "silver ring", nameFormatted = "silver" + "\n" + "ring", description = "a silver ring", slot = Slot.Ring, visualsObject = silverRingVisualsObject, shopCoinCost = 5 });
        //equipmentDatas.Add(Equipment.GoldRing, new EquipmentData { name = "gold ring", nameFormatted = "gold" + "\n" + "ring", description = "a gold ring", slot = Slot.Ring, visualsObject = goldRingVisualsObject, shopCoinCost = 5 });
        //equipmentDatas.Add(Equipment.CopperBracelet, new EquipmentData { name = "copper bracelet", nameFormatted = "copper" + "\n" + "bracelet", description = "a copper bracelet", slot = Slot.Bracelet, visualsObject = copperBraceletVisualsObject, shopCoinCost = 5 });
        //equipmentDatas.Add(Equipment.SilverBracelet, new EquipmentData { name = "silver bracelet", nameFormatted = "silver" + "\n" + "bracelet", description = "a silver bracelet", slot = Slot.Bracelet, visualsObject = silverBraceletVisualsObject, shopCoinCost = 5 });
        //equipmentDatas.Add(Equipment.GoldBracelet, new EquipmentData { name = "gold bracelet", nameFormatted = "gold" + "\n" + "bracelet", description = "a golden bracelet", slot = Slot.Bracelet, visualsObject = goldBraceletVisualsObject, shopCoinCost = 5 });
        //equipmentDatas.Add(Equipment.GreenCloak, new EquipmentData { name = "green cloak", nameFormatted = "green" + "\n" + "cloak", description = "a green cloak", slot = Slot.Body, visualsObject = greenCloakVisualsObject, shopCoinCost = 5, matA = greenCloakMatA, matB = greenCloakMatB, matC = greenCloakMatC });
        equipmentDatas.Add(Equipment.WhiteCloak, new EquipmentData { name = "white cloak", nameFormatted = "white" + "\n" + "cloak", description = "a white cloak" + "\n" + "a gift from the moon", slot = Slot.Body, visualsObject = whiteCloakVisualsObject, shopCoinCost = 5, matA = whiteCloakMatA, matB = whiteCloakMatB, matC = whiteCloakMatC });
        equipmentDatas.Add(Equipment.BlueCloak, new EquipmentData { name = "blue cloak", nameFormatted = "blue" + "\n" + "cloak", description = "a velvet cloak" + "\n" + "worn by nobility", slot = Slot.Body, visualsObject = blueCloakVisualsObject, shopCoinCost = 5, matA = blueCloakMatA, matB = blueCloakMatB, matC = blueCloakMatC });
        //equipmentDatas.Add(Equipment.BronzeSword, new EquipmentData { name = "bronze sword", nameFormatted = "bronze" + "\n" + "sword", description = "a bronze sword", slot = Slot.Weapon, visualsObject = bronzeSwordVisualsObject, shopCoinCost = 5 });
        //equipmentDatas.Add(Equipment.SilverSword, new EquipmentData { name = "silver sword", nameFormatted = "silver" + "\n" + "sword", description = "a silver sword", slot = Slot.Weapon, visualsObject = silverSwordVisualsObject, shopCoinCost = 5 });
        equipmentDatas.Add(Equipment.WoodenClub, new EquipmentData { name = "wooden club", nameFormatted = "wooden" + "\n" + "club", description = "a wooden club", slot = Slot.Weapon, visualsObject = woodenClubVisualsObject, shopCoinCost = 5 });
        equipmentDatas.Add(Equipment.BasicWand, new EquipmentData { name = "basic wand", nameFormatted = "basic" + "\n" + "wand", description = "a basic wand", slot = Slot.Weapon, visualsObject = basicWandVisualsObject, shopCoinCost = 5 });
        equipmentDatas.Add(Equipment.Torch, new EquipmentData { name = "torch", nameFormatted = "basic" + "\n" + "wand", description = "a basic torch", slot = Slot.Weapon, visualsObject = torchVisualsObject, shopCoinCost = 5 });
    }

    public Equipment GetEquipmentFromIndex ( int _index )
    {
        return (Equipment)(_index);
    }

    public EquipmentData GetEquipmentData ( Equipment _equipment )
    {
        EquipmentData ret;
        equipmentDatas.TryGetValue(_equipment, out ret);
        return ret;
    }

    [System.Serializable]
    public struct SpecialsPackage
    {
        public Specials special;
        public float chance;

        public SpecialsPackage ( Specials _special, float _chance )
        {
            special = _special;
            chance = _chance;
        }
    };

    [System.Serializable]
    public struct EquipmentStats
    {
        public float moveSpeedAdd;
        public float attackSpeedAdd;
        public float knockbackAdd;
        public int meleeDamageAdd;
        public int magicDamageAdd;
        public int maxHealthAdd;
        public int maxFireAdd;
        public float alertDstAdd;
    };

    [System.Serializable]
    public struct EquipmentData
    {
        public string name;
        public string nameFormatted;
        public string description;
        public Slot slot;

        public GameObject visualsObject;

        public float shopSclFactorAdd;
        public Vector3 shopPosOffAdd;
        public Vector3 shopRotOffAdd;
        public int shopCoinCost;

        public Material matA;
        public Material matB;
        public Material matC;

        public EquipmentStats stats;
        public List<SpecialsPackage> specialsPackage;
    };
}
