using UnityEngine;
using System.Collections.Generic;

public class BlessingDatabase : MonoBehaviour
{
    // instance
    public static BlessingDatabase instance;

    // values
    [HideInInspector] public int blessingCountMax;
    [HideInInspector] public int tankHealthAdd;
    [HideInInspector] public int glassCannonHealthSubtract;
    [HideInInspector] public int glassCannonDamageBoost;
    [HideInInspector] public int secondChanceHealthGain;
    [HideInInspector] public int warriorDamageBoost;
    [HideInInspector] public int sorcererDamageBoost;
    [HideInInspector] public float wildFireRadiusAdd;
    [HideInInspector] public int swiftStrikesFrameReduce;

    // blessings
    public enum Blessing
    {
        SwiftStrikes, // faster melee attacks
        QuickFeet, // higher movement speed
        GlassCannon, // -3 max health, but +1 damage output (all attacks)
        FireBurst, // fire explodes when collected, dealing damage to nearby enemies
        Warrior, // +1 to all melee attacks
        Sorcerer, // +1 to all fire attacks
        Tank, // +3 max health
        SecondChance, // +5 health when under 0 health, can only be used once
        Lucky, // more likely to find coins
        Mournful, // enemies have a chance of dropping an additional tear when defeated
        WildFire, // bigger explosions from fire attacks
        AgileJumper, // take no damage when mid-air
        Acrobat, // can attack when in mid-air
        Spray, // shoot double fire, but fire less strong
        FireRage, // melee weapon shoots fire when on 1 health
        Revenge, // gain 1 fire when damaged
        Hungry, // more likely to find donuts
        Resilient, // can exceed max health
        Thrifty, // items in the shop are cheaper
        Drainer, // 10 percent chance to gain 1 health when defeating enemies
        HotFire, // +1 to all fire attacks, but fire attacks have more knockback
    };

    // dictionary
    public Dictionary<Blessing,BlessingData> blessingDatas;

    void Awake ()
    {
        instance = this;

        // init
        Init();
    }

    void Init ()
    {
        // values
        tankHealthAdd = 2;
        glassCannonHealthSubtract = 2;
        glassCannonDamageBoost = 1;
        secondChanceHealthGain = 3;
        warriorDamageBoost = 2;
        sorcererDamageBoost = 1;
        wildFireRadiusAdd = 1f;
        swiftStrikesFrameReduce = 6;

        // create dictionary
        blessingDatas = new Dictionary<Blessing, BlessingData>();
        blessingDatas.Add(Blessing.SwiftStrikes, new BlessingData { name = "swift" + "\n" + "strikes", description = "faster melee attacks" });
        blessingDatas.Add(Blessing.Warrior, new BlessingData { name = "warrior", description = "+" + warriorDamageBoost.ToString() + " to all melee attacks" + "\n" + "-1 to all fire attacks" });
        blessingDatas.Add(Blessing.Sorcerer, new BlessingData { name = "sorcerer", description = "+" + sorcererDamageBoost.ToString() + " to all fire attacks" + "\n" + "-1 to all melee attacks" });
        blessingDatas.Add(Blessing.GlassCannon, new BlessingData { name = "glass" + "\n" + "cannon", description = "-" + glassCannonHealthSubtract.ToString() + " max health, but +" + glassCannonDamageBoost.ToString() + " damage" + "\n" + "output to all attacks" });
        blessingDatas.Add(Blessing.QuickFeet, new BlessingData { name = "olympic" + "\n" + "flame", description = "higher movement speed" });
        blessingDatas.Add(Blessing.FireBurst, new BlessingData { name = "fire" + "\n" + "burst", description = "fire explodes when collected," + "\n" + "dealing damage to nearby enemies" });
        blessingDatas.Add(Blessing.Tank, new BlessingData { name = "tank", description = "+" + tankHealthAdd.ToString() + " max health" });
        blessingDatas.Add(Blessing.SecondChance, new BlessingData { name = "second" + "\n" + "chance", description = "+" + secondChanceHealthGain.ToString() + " health when reaching 0 health," + "\n" + "only triggers once" });
        blessingDatas.Add(Blessing.Lucky, new BlessingData { name = "lucky", description = "more likely to find coins" });
        blessingDatas.Add(Blessing.Mournful, new BlessingData { name = "mournful", description = "enemies have a chance of shedding" + "\n" + "an additional tear when defeated" });
        blessingDatas.Add(Blessing.WildFire, new BlessingData { name = "wild" + "\n" + "fire", description = "bigger explosions from " + "\n" + " fire attacks" });
        blessingDatas.Add(Blessing.AgileJumper, new BlessingData { name = "agile" + "\n" + "jumper", description = "take no damage from attacks" + "\n" + "when in mid-air" });
        blessingDatas.Add(Blessing.Acrobat, new BlessingData { name = "acrobat", description = "can attack when in mid-air" });
        blessingDatas.Add(Blessing.Spray, new BlessingData { name = "spray", description = "shoot double fire" + "\n" + "but it's less strong" });
        blessingDatas.Add(Blessing.FireRage, new BlessingData { name = "flaming" + "\n" + "blade", description = "melee weapon shoots fire" + "\n" + "when on 1 health" });
        blessingDatas.Add(Blessing.Revenge, new BlessingData { name = "revenge", description = "gain 1 fire when damaged" });
        blessingDatas.Add(Blessing.Hungry, new BlessingData { name = "hungry", description = "more likely to find donuts" });
        blessingDatas.Add(Blessing.Resilient, new BlessingData { name = "resilient", description = "can exceed max health" });
        blessingDatas.Add(Blessing.Thrifty, new BlessingData { name = "thrifty", description = "items in the shop are cheaper" });
        blessingDatas.Add(Blessing.Drainer, new BlessingData { name = "drainer", description = "5 percent chance to gain" + "\n" + "1 health when defeating enemies" });
        blessingDatas.Add(Blessing.HotFire, new BlessingData { name = "hot" + "\n" + "fire", description = "+1 to all fire damage" + "\n" + "but more knockback on fire attacks" });
        blessingCountMax = blessingDatas.Count;
    }

    public Blessing GetBlessingFromIndex ( int _index )
    {
        return (Blessing)(_index);
    }

    public BlessingData GetBlessingData ( Blessing _blessing )
    {
        BlessingData ret;
        blessingDatas.TryGetValue(_blessing, out ret);
        return ret;
    }

    [System.Serializable]
    public struct BlessingData
    {
        public string name;
        public string description;
    };
}
