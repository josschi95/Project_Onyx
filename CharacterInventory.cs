using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : Inventory
{
    protected EquipmentManager equipmentManager;
    protected CharacterCombat characterCombat;
    protected CharacterController characterController;

    //If i'm moving things, this would probably go to Combat
    public List<Ability> abilities = new List<Ability>();
    //No clue where this one would go
    public List<Recipe> cookBook = new List<Recipe>();
    //Honestly wondering if this wouldn't be better held in characterSpellcasting

    public float carryWeight { get; protected set; }
    public float netCarryWeight { get; protected set; }
    public Item lockpick;
    public int lockpicks;

    protected override void Start()
    {
        base.Start();
        equipmentManager = GetComponent<EquipmentManager>();
        characterCombat = GetComponent<CharacterCombat>();
        characterController = GetComponent<CharacterController>();
        onItemChangedCallback += UpdateCarryWeight;
    }

    protected virtual void UpdateCarryWeight()
    {
        int num = copperPieces + silverPieces + goldPieces;
        netCarryWeight = carryWeight + (num * 0.01f);
        if (netCarryWeight >= characterController.characterStats.statSheet.carryCapacity.GetValue())
            characterController.overburdened = true;
        else characterController.overburdened = false;
    }

    public override void AddItem(Item item, int quantity)
    {
        if (item.name == "Lockpick") lockpicks += quantity;
        carryWeight += item.weight * quantity;

        base.AddItem(item, quantity);
    }

    public override void RemoveItem(Item item, int quantity)
    {
        if (item.name == "Lockpick") lockpicks -= quantity;
        carryWeight -= item.weight * quantity;

        base.RemoveItem(item, quantity);
    }

    public override void ClearInventory()
    {
        base.ClearInventory();
        carryWeight = 0;
        UpdateCarryWeight();
    }

    protected override void RemoveFromList(InventoryItem inventoryItem, List<InventoryItem> list)
    {
        base.RemoveFromList(inventoryItem, list);
        if (inventoryItem.isEquipped) inventoryItem.UnequipItem(characterController);
    }
}