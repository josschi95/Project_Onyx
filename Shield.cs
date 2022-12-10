using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Weapon/Shield")]
public class Shield : Weapon
{
    [Header("Shield Properties")]
    public bool isMagic = false;
    public bool isMetal;
    [TextArea(3, 5)] public string notes = "I need some other property in here so better shields improve blocking and it's not entirely based on skill score";

    private void Reset()
    {
        equipSlot = EquipmentSlot.Primary_Off;
        itemCategory = ItemCategory.WEAPON;
        weaponType = WeaponType.Shield;
    }
}