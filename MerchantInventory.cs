using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class MerchantInventory : Inventory
{
    [HideInInspector] public NPC_Character actor;
    [Header("Merchant Properties")]
    //The items that the merchant will have for sale on a daily basis
    public List<InventoryItem> masterItemList = new List<InventoryItem>();
    //Items that the player sold to the merchant which can be bought back for same price on the same day
    public List<InventoryItem> recentlyPurchased = new List<InventoryItem>();

    protected override void Start()
    {
        CompileInventories();
        DateTimeManager.instance.onNewDay += CompileInventories;
        base.Start();
        actor = GetComponent<NPC_Character>();
    }

    //Call this function daily to reset all of the merchant's goods
    public void CompileInventories()
    {
        ClearInventory();

        for (int i = 0; i < masterItemList.Count; i++)
        {
            //AddItem(masterItemList[i].item, masterItemList[i].quantity);
            Item item = DatabaseManager.GetItem(masterItemList[i].itemID);
            AddItem(item, masterItemList[i].quantity);

            //Yes I have to do it this way because otherwise it goes into recentlyPurchased
            //switch (masterItemList[i].item.itemCategory)
            switch (item.itemCategory)
            {
                case ItemCategory.APPAREL:
                    {
                        //AddMethod(masterItemList[i].item, masterItemList[i].quantity, apparel);
                        AddMethod(item, masterItemList[i].quantity, apparel);
                        break;
                    }
                case ItemCategory.WEAPON:
                    {
                        //AddMethod(masterItemList[i].item, masterItemList[i].quantity, weapons);
                        AddMethod(item, masterItemList[i].quantity, weapons);
                        break;
                    }
                case ItemCategory.POTION:
                    {
                        //AddMethod(masterItemList[i].item, masterItemList[i].quantity, potions);
                        AddMethod(item, masterItemList[i].quantity, potions);
                        break;
                    }
                /*case ItemCategory.CRAFTING:
                    {
                        //AddMethod(masterItemList[i].item, masterItemList[i].quantity, crafting);
                        AddMethod(item, masterItemList[i].quantity, crafting);
                        break;
                    }*/
                case ItemCategory.MISC:
                    {
                        //AddMethod(masterItemList[i].item, masterItemList[i].quantity, miscellaneous);
                        AddMethod(item, masterItemList[i].quantity, miscellaneous);
                        break;
                    }
            }
        }
    }

    public override void ClearInventory()
    {
        base.ClearInventory();
        recentlyPurchased.Clear();
    }

    public void OnBeginShopping()
    {
        UIManager.instance.OpenDoubleInventories(this, InventoryInteraction.Bartering);
        DialogueManager.SetDialoguePanel(false, false);
        //may need to do some other stuff in here
    }

    public void OnEndShopping()
    {
        DialogueManager.SetDialoguePanel(true);
        UIManager.instance.CloseBarteringInventory();
        //at this point I could also move recentlyPurchased to regular inventories
    }

    public override void AddItem(Item item, int quantity)
    {
        AddMethod(item, quantity, recentlyPurchased);
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public override void RemoveItem(Item item, int quantity)
    {
        foreach (InventoryItem merchantItem in recentlyPurchased)
        {
            if (merchantItem.itemID == item.itemID)
            //if (merchantItem.item == item)
            {
                RemoveMethod(item, quantity, recentlyPurchased);
                return;
            }
        }
        base.RemoveItem(item, quantity);
    }

    public float GetPriceModifier()
    {
        //factor in player's barter skill
        //NPC's affinity for the player, player position in faction (if relevant),
        //player's local rep, and some perk I'll probably add at some point
        float barter = PlayerStats.instance.statSheet.barter.GetValue();
        float modifier = (4 - (2 * (barter / 100))) * actor.character.residence.generalLocalCostMultiplier;

        float bonusMods = ((100 + actor.character.playerAffinity) / 100)
           * ((100 + (actor.character.residence.playerReputation * 0.25f)) / 100);
         //* ((100 + (actor.character.faction.playerRep * 0.5f)) / 100)
         //* ((100 + somePerk) / 100)

        modifier /= bonusMods;
        return modifier;
    }
}
