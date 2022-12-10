using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon/Weapon")]
public class Weapon : Equipment
{
    void Reset()
    {
        equipSlot = EquipmentSlot.Primary_Main;
        itemCategory = ItemCategory.WEAPON;
    }

    [Header("Weapon Properties")]
    public float reach = 1f;
    public AttackDirection defaultAttack;
    public Vector3 secondaryPosition;
    public Vector3 secondaryRotation;
    [Space]
    public string weaponClass; 
    public WeaponType weaponType;
    public WeaponSkill weaponSkill;
    [Space]

    [Header("Weapon Damage")]
    public DamageType bashDamage;
    public int minBashDmg;
    public int maxBashDmg;
    public DamageType slashDamage;
    public int minSlashDmg;
    public int maxSlashDmg;
    public DamageType thrustDamage;
    public int minThrustDmg;
    public int maxThrustDmg;

    [Space]
    [Range(0.5f, 2)]
    public float weaponSpeed = 1;
    [Range(1, 10)]
    public float critChance = 1f;

    [Space]
    public bool isFinesseWeapon = false;
    public bool isLethal = true;
    public bool canBePoisoned = false;

    public override bool IsEquipped(EquipmentManager equip)
    {
        if (equip == null) return false;
        for (int i = 0; i < 4; i++)
        {
            if (equip.currentEquipment[i] != null && DatabaseManager.GetItem(equip.currentEquipment[i].itemID) == this) return true;
            //if (equip.currentEquipment[i] != null && equip.currentEquipment[i].item == this) return true;
        }

        return false;
    }
}
//Would be nice if these lined up somehow... job for another day
public enum WeaponType { Shield, One_Handed, Heavy, Pole, Bow, Arrow, Thrown } //Would very much like for arrow/thrown to somehow combine
public enum WeaponSkill { Axe, Blade, Club, Polearm, Shield, Striker, Ranged }
public enum AttackDirection { Bash, Slash, Thrust }

// "Hand Axe, Axe, GreatAxe, Club, Greatclub, Mace, Maul, Throwing Knife, Dagger, Shortsword, Longsword, Greatsword, Javelin, Spear, Halberd, Shield, Unarmed, Bow, Scepter, Staff, Wand, Arrow";