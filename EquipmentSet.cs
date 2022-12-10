using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Set", menuName = "Inventory/Equip Set")]

public class EquipmentSet : ScriptableObject
{
    public string setName;
    public Equipment[] setPieces;
    //public int halfSetCount;
    //public EffectHolder[] halfSetBonus;
    public EffectHolder[] fullSetBonus;
}
