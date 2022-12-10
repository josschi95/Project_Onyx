using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerTransactionDisplay : InventoryDisplay
{
    public TMP_Text carryCapacityText, coinageText;
    [HideInInspector] public Inventory otherInventory;

    private PlayerInventory playerInventory;
    private PlayerEquipmentManager playerEquipment;
    private PlayerStats playerStats;
    private PlayerController playerController;

    protected override void Start()
    {
        playerEquipment = PlayerEquipmentManager.instance;
        playerController = PlayerController.instance;
        playerInventory = PlayerInventory.instance;
        inventory = playerInventory;
        playerStats = PlayerStats.instance;
        base.Start();
    }

    public void OnStartTransaction(Inventory newInventory, InventoryInteraction type)
    {
        otherInventory = newInventory;
        inventoryInteraction = type;
        playerInventory.onItemChangedCallback += UpdatePanel;

        costModifier = 1;
        if (newInventory is MerchantInventory merchant)
        {
            costModifier = (1 / merchant.GetPriceModifier());
            Debug.Log("Player: " + costModifier);
        }

        ResetOrder();
        UpdatePanel();
    }

    public void OnEndTransaction()
    {
        playerInventory.onItemChangedCallback -= UpdatePanel;
        ClearInventoryPanelItems();
        otherInventory = null;
    }

    public override void UpdatePanel()
    {
        carryCapacityText.text = "Carry Capacity: " + playerInventory.netCarryWeight + "/" + playerStats.statSheet.carryCapacity.GetValue().ToString();
        coinageText.text = playerInventory.copperPieces + "c, " + playerInventory.silverPieces + "s, " + playerInventory.goldPieces + "g";
        base.UpdatePanel();
    }

    protected override void ButtonSettings(InventoryItemUI txtItem, InventoryItem inventoryItem)
    {
        Item item = DatabaseManager.GetItem(inventoryItem.itemID);
        //txtItem.item = inventoryItem.item;
        txtItem.item = item;
        txtItem.spell = null;
        //txtItem.itemImage.sprite = inventoryItem.item.icon;
        txtItem.itemImage.sprite = item.icon;
        txtItem.equippedFrame.enabled = inventoryItem.isEquipped;

        if (inventoryInteraction == InventoryInteraction.Storage)
        {
            SetButtonToTransfer(txtItem, inventoryItem);
        }
        else if (inventoryInteraction == InventoryInteraction.Bartering)
        {
            SetButtonToSell(txtItem, inventoryItem);
        }
        else Debug.Log("ERROR");
    }

    private void SetButtonToTransfer(InventoryItemUI txtItem, InventoryItem inventoryItem)
    {
        if (otherInventory == null)
        {
            Debug.LogWarning("other inventory null");
            return;
        }
        txtItem.itemButton.onClick.AddListener(delegate { TransferStorage(inventoryItem, otherInventory); });
    }

    private void SetButtonToSell(InventoryItemUI txtItem, InventoryItem inventoryItem)
    {
        if (otherInventory == null) return;
        txtItem.itemButton.onClick.AddListener(delegate { SellItem(inventoryItem, otherInventory); });
    }
}
