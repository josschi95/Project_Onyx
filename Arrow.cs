using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arrow", menuName = "Inventory/Weapon/Arrow")]
public class Arrow : Weapon
{
    private void Reset()
    {
        itemCategory = ItemCategory.WEAPON;
        equipSlot = EquipmentSlot.Primary_Off;
        isStackable = true;
    }
    public GameObject arrowPrefab;
}
