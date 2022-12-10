using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InteractionInventoryDisplay : InventoryDisplay
{
    private PlayerInventory playerInventory;
    public Button takeAllButton, takeCoinsButton, closeButton;
    public TMP_Text coinageText, titleText;

    public delegate void OnInteractionEnd();
    public OnInteractionEnd onInteractionEnd;

    protected override void Start()
    {
        playerInventory = PlayerInventory.instance;
        base.Start();
    }

    public void OnStartTransaction(Inventory newInventory, InventoryInteraction type)
    {
        takeAllButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
        costModifier = 1;
        inventory = newInventory;
        inventoryInteraction = type;

        if (type == InventoryInteraction.Looting)
        {
            transform.localScale = new Vector3(0.8f, 0.8f);

            takeAllButton.gameObject.SetActive(true);
            takeCoinsButton.gameObject.SetActive(true);
            takeAllButton.onClick.AddListener(delegate { TakeAll(); });
            takeCoinsButton.onClick.AddListener(delegate { TakeAllCoinage(); });

            closeButton.onClick.AddListener(delegate { 
                UIManager.instance.CloseLootContainer(); });
        }
        else
        {
            if (type == InventoryInteraction.Storage)
            {
                takeAllButton.gameObject.SetActive(true);
                takeCoinsButton.gameObject.SetActive(true);
                takeAllButton.onClick.AddListener(delegate { TakeAll(); });
                takeCoinsButton.onClick.AddListener(delegate { TakeAllCoinage(); });
                closeButton.onClick.AddListener(delegate { 
                    UIManager.instance.CloseDoubleInventories(); });
            }
            else if (newInventory is MerchantInventory newMerchant)
            {
                costModifier = newMerchant.GetPriceModifier();
                Debug.Log("Merchant: " + costModifier);

                takeAllButton.gameObject.SetActive(false);
                takeCoinsButton.gameObject.SetActive(false);
                takeAllButton.onClick.RemoveAllListeners();
                takeCoinsButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(newMerchant.OnEndShopping);
            }

            transform.localScale = new Vector3(1f, 1f);
        }

        titleText.text = newInventory.transform.name;
        inventory.onItemChangedCallback += UpdatePanel;
        ResetOrder();
        UpdatePanel();
    }

    public void OnEndTransaction()
    {
        if (onInteractionEnd != null)
            onInteractionEnd.Invoke();

        if (inventory != null) 
            inventory.onItemChangedCallback -= UpdatePanel;

        ClearInventoryPanelItems();
        inventory = null;
    }

    public override void UpdatePanel()
    {
        if (inventoryInteraction == InventoryInteraction.Looting && inventory.isEmpty)
        {
            UIManager.instance.CloseLootContainer();
            return;
        }

        coinageText.text = inventory.copperPieces + "c, " + inventory.silverPieces + "s, " + inventory.goldPieces + "g";

        if (activePanel == 6)
        {
            if (inventory is MerchantInventory merch)
                DisplayBuyBackCategory(merch);
            else
            {
                Debug.Log("ERROR");
                activePanel = 0;
                UpdatePanel();
            }
        }
        base.UpdatePanel();
    }

    public void DisplayBuyBackCategory(MerchantInventory inventory)
    {
        activePanel = 6;

        itemInfoText1.text = "";
        itemInfoText2.text = "TYPE";
        itemInfoText3.text = "WEIGHT";
        itemInfoText4.text = "VALUE";
        info_1_Button.interactable = false;

        SortList(inventory.recentlyPurchased);
        PopulatePanel(inventory.recentlyPurchased);
    }

    protected override void ButtonSettings(InventoryItemUI txtItem, InventoryItem inventoryItem)
    {
        Item item = DatabaseManager.GetItem(inventoryItem.itemID);
        //txtItem.item = inventoryItem.item;
        txtItem.item = item;
        txtItem.spell = null;
        //txtItem.itemImage.sprite = inventoryItem.item.icon;
        txtItem.itemImage.sprite = item.icon;
        txtItem.equippedFrame.enabled = false;

        Button butt = txtItem.itemButton; //lol
        if (inventoryInteraction == InventoryInteraction.Looting)
        {
            //butt.onClick.AddListener(delegate { TransferItems(inventoryItem.item, inventoryItem.quantity, playerInventory, false); });
            butt.onClick.AddListener(delegate { TransferItems(item, inventoryItem.quantity, playerInventory, false); });
        }
        else if (inventoryInteraction == InventoryInteraction.Storage)
        {
            butt.onClick.AddListener(delegate { TransferStorage(inventoryItem, playerInventory); });
        }
        else if (inventoryInteraction == InventoryInteraction.Bartering)
        {
            butt.onClick.AddListener(delegate { SellItem(inventoryItem, playerInventory); });
        }
        else Debug.LogWarning("ERROR");
    }

    public void TakeAll()
    {
        TakeAllCoinage();
        //var newList = inventory.apparel.Union(inventory.weapons).Union(inventory.potions).Union(inventory.crafting).Union(inventory.miscellaneous).ToList();
        var newList = inventory.apparel.Union(inventory.weapons).Union(inventory.potions).Union(inventory.miscellaneous).ToList();
        for (int i = 0; i < newList.Count; i++)
        {
            //TransferItems(newList[i].item, newList[i].quantity, playerInventory, false);
            TransferItems(DatabaseManager.GetItem(newList[i].itemID), newList[i].quantity, playerInventory, false);
        }

        if (inventoryInteraction == InventoryInteraction.Storage)
            UIManager.instance.CloseDoubleInventories();
    }

    public void TakeAllCoinage()
    {
        playerInventory.AddCurrency(CurrencyType.Copper, inventory.copperPieces);
        playerInventory.AddCurrency(CurrencyType.Silver, inventory.silverPieces);
        playerInventory.AddCurrency(CurrencyType.Gold, inventory.goldPieces);

        inventory.copperPieces = 0;
        inventory.silverPieces = 0;
        inventory.goldPieces = 0;
    }
}