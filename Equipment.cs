using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    [Header("Equipment Properties")]
    public EquipmentSlot equipSlot;
    public EquipmentSet setPiece;
    [Space]
    public EffectHolder[] equipEffects;

    public override bool IsEquipped(EquipmentManager equip)
    {
        if (equip == null) return false;

        for (int i = 0; i < equip.currentEquipment.Length; i++)
            if (equip.currentEquipment[i] != null && equip.currentEquipment[i].itemID == this.itemID) return true;

        //for (int i = 0; i < equip.currentEquipment.Length; i++)
            //if (equip.currentEquipment[i] != null && equip.currentEquipment[i].item == this) return true;

        return false;
    }
}
public enum EquipmentSlot { Primary_Main, Primary_Off, Secondary_Main, Secondary_Off, Head, Body, Hands, Feet, Back, Ring_Left, Ring_Right, Amulet }