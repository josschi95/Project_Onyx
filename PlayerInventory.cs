using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : CharacterInventory
{
    #region - Singleton -
    public static PlayerInventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerInventory found");
            return;
        }

        instance = this;
    }
    #endregion

    protected override void UpdateCarryWeight()
    {
        base.UpdateCarryWeight();
        if (characterController.overburdened)
        {
            UIManager.instance.AddNotification("You are Overburdened");
        }
    }

    public void LearnAbility(Ability ability)
    {
        if (abilities.Contains(ability))
            UIManager.instance.DisplayPopup(ability.name + " is already known");
        else
        {
            UIManager.instance.DisplayPopup("You learned " + ability.name);
            abilities.Add(ability);
        }
    }

    public void LearnRecipe(Recipe recipe)
    {
        if (cookBook.Contains(recipe))
            UIManager.instance.DisplayPopup(recipe.name + " is already known");
        else
        {
            UIManager.instance.DisplayPopup("You learned the " + recipe.name);
            cookBook.Add(recipe);
        }
    }

    public override void AddItem(Item item, int quantity)
    {
        base.AddItem(item, quantity);
        if (quantity > 1) UIManager.instance.AddNotification(item.name + " (" + quantity + ") added");
        else UIManager.instance.AddNotification(item.name + " added");
    }

    public override void RemoveItem(Item item, int quantity)
    {
        base.RemoveItem(item, quantity);

        //Don't display message if the item is equipped, likely only ammunition
        if (item.IsEquipped(characterController.equipmentManager) == false)
        {
            if (quantity > 1) UIManager.instance.AddNotification(item.name + " (" + quantity + ") removed");
            else UIManager.instance.AddNotification(item.name + " removed");
        }
    }

    protected override void RemoveFromList(InventoryItem inventoryItem, List<InventoryItem> list)
    {
        base.RemoveFromList(inventoryItem, list);

        //if (inventoryItem.isEquipped && inventoryItem.item is Potion potion)
        if (inventoryItem.isEquipped && DatabaseManager.GetItem(inventoryItem.itemID) is Potion potion)
        {
            if (HUDManager.instance.quickslotManager.preparedPotions.Contains(potion))
                HUDManager.instance.quickslotManager.OnPotionsChange(potion);
        }
    }
}
