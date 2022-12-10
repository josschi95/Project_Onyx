using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A base class for all potions to derive from */

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potions/Potion")]
public class Potion : Item
{
    private void Reset()
    {
        itemCategory = ItemCategory.POTION;
        currencyType = CurrencyType.Gold;
        weight = 1;
    }

    [Header("Potion Properties")]
    public PotionType potionType;
    public PotionClass potionClass;
    public List<EffectHolder> potionEffects = new List<EffectHolder>();

    public override bool IsEquipped(EquipmentManager equip)
    {
        if (equip is PlayerEquipmentManager)
        {
            if (HUDManager.instance.quickslotManager.preparedPotions.Contains(this)) 
                return true;
        }
        return false;
    }

    public void OnPotionCreation()
    {
        //Call this function when a potion is crafted
        float num = 0;
        for (int i = 0; i < potionEffects.Count; i++)
        {
            float effectCost = potionEffects[i].spellEffect.baseCost * (potionEffects[i].magnitude * (potionEffects[i].duration + 1));
            num += effectCost;
        }
        int newNum = Mathf.RoundToInt(num);
        baseValue = newNum;
    }
}

public enum PotionType { Potion, Poison }
public enum PotionClass { Health, Stamina, Mana, Attribute, Skill, Resistance, Poison, Custom }