using UnityEngine;
using System.Collections.Generic;

public class WeaponDatabase : MonoBehaviour
{
    public static WeaponDatabase instance;

    public enum Weapon { IronSword };

    public Dictionary<Weapon,GameObject> weaponObjectReference;

    void Awake ()
    {
        instance = this;

        // object references
        CreateObjectReferences();
    }

    void CreateObjectReferences ()
    {
        weaponObjectReference = new Dictionary<Weapon, GameObject>();
        weaponObjectReference.Add(Weapon.IronSword,PrefabManager.instance.ironSwordPrefab);
    }
}
