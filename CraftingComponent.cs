using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafting Component", menuName = "Crafting/Component")]
public class CraftingComponent : Item
{
    private void Reset()
    {
        itemCategory = ItemCategory.CRAFTING;
        isStackable = true;
    }

    public CraftingType type;
}

//public enum ComponentQuality { Common, Uncommon, Rare, etc. }
