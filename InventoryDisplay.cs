using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class InventoryDisplay : MonoBehaviour
{
    public InventoryInteraction inventoryInteraction;
    [Space]
    [Tooltip("Root for inventory items")]
    public Transform InventoryPanelItem;
    [Tooltip("prefab representing inventory item UI")]
    public QuantitySelectionPanel quantitySelect;

    [Header("Buttons")]
    public Button categoryAll; //categoryBooksandKeys;
    public Button categoryApparel, categoryWeapons, categoryPotions, categoryCrafting, categoryMisc;
    public Button nameButton, info_1_Button, info_2_Button, info_3_Button, info_4_Button;
    [Space]
    public TMP_Text itemInfoText1;
    public TMP_Text itemInfoText2, itemInfoText3, itemInfoText4;

    protected int activePanel;
    protected float costModifier; //Modifier to prices when buying/selling
    [HideInInspector] public int sortOrder;

    [Space]
    public Inventory inventory;

    protected virtual void Start()
    {
        categoryAll.onClick.AddListener(() => { DisplayAllCategories(inventory); });
        categoryApparel.onClick.AddListener(() => { DisplayApparelCategory(inventory); });
        categoryWeapons.onClick.AddListener(() => { DisplayWeaponsCategory(inventory); });
        categoryPotions.onClick.AddListener(() => { DisplayPotionsCategory(inventory); });
        //categoryCrafting.onClick.AddListener(() => { DisplayCraftingCategory(inventory); });
        categoryMisc.onClick.AddListener(() => { DisplayMiscCategory(inventory); });

        nameButton.onClick.AddListener(() =>
        {
            sortOrder = 0;
            UpdatePanel();
        });
        info_1_Button.onClick.AddListener(() =>
        {
            sortOrder = 1;
            UpdatePanel();
        });
        info_2_Button.onClick.AddListener(() =>
        {
            sortOrder = 2;
            UpdatePanel();
        });
        info_3_Button.onClick.AddListener(() =>
        {
            sortOrder = 3;
            UpdatePanel();
        });
        info_4_Button.onClick.AddListener(() =>
        {
            sortOrder = 4;
            UpdatePanel();
        });
    }

    public virtual void ResetOrder()
    {
        sortOrder = 0;
    }

    public virtual void UpdatePanel()
    {
        if (activePanel == 0)
        {
            DisplayAllCategories(inventory);
        }
        else if (activePanel == 1)
        {
            DisplayApparelCategory(inventory);
        }
        else if (activePanel == 2)
        {
            DisplayWeaponsCategory(inventory);
        }
        else if (activePanel == 3)
        {
            DisplayPotionsCategory(inventory);
        }
        else if (activePanel == 4)
        {
            //DisplayCraftingCategory(inventory);
        }
        else if (activePanel == 5)
        {
            DisplayMiscCategory(inventory);
        }
    }

    #region - Category Displays -
    protected virtual void SortList(List<InventoryItem> list)
    {
        switch (sortOrder)
        {
            case 0:
                {
                    list.OrderBy(x => DatabaseManager.GetItem(x.itemID).name).ToList();
                    //list.OrderBy(x => x.item.name).ToList();
                    break;
                }
            case 1:
                {
                    list.OrderBy(x => ItemClass(x)).ToList();
                    break;
                }
            case 2:
                {
                    list.OrderBy(x => ItemMagnitude(x)).ToList();
                    break;
                }
            case 3:
                {
                    list.OrderBy(x => DatabaseManager.GetItem(x.itemID).weight).ToList();
                    //list.OrderBy(x => x.item.weight).ToList();
                    break;
                }
            case 4:
                {
                    list.OrderBy(x => DatabaseManager.GetItem(x.itemID).baseValue).ToList();
                    //list.OrderBy(x => x.item.baseValue).ToList();
                    break;
                }
        }
    }

    protected virtual void DisplayAllCategories(Inventory inventory)
    {
        activePanel = 0;

        itemInfoText1.text = "TYPE";
        itemInfoText2.text = "MAGN.";
        itemInfoText3.text = "WEIGHT";
        itemInfoText4.text = "VALUE";
        info_1_Button.interactable = true;

        //var newList = inventory.apparel.Union(inventory.weapons).Union(inventory.potions).Union(inventory.crafting).Union(inventory.miscellaneous).ToList();
        var newList = inventory.apparel.Union(inventory.weapons).Union(inventory.potions).Union(inventory.miscellaneous).ToList();

        switch (sortOrder)
        {
            case 0:
                {
                    newList = newList.OrderBy(go => DatabaseManager.GetItem(go.itemID).name).ToList();
                    //newList = newList.OrderBy(go => go.item.name).ToList();
                    break;
                }
            case 3:
                {
                    newList = newList.OrderBy(go => DatabaseManager.GetItem(go.itemID).itemCategory).ToList();
                    //newList = newList.OrderBy(go => go.item.itemCategory).ToList();
                    break;
                }
            case 4:
                {
                    newList = newList.OrderBy(go => DatabaseManager.GetItem(go.itemID).weight).ToList();
                    //newList = newList.OrderBy(go => go.item.weight).ToList();
                    break;
                }
            case 5:
                {
                    newList = newList.OrderBy(go => DatabaseManager.GetItem(go.itemID).baseValue).ToList();
                    //newList = newList.OrderBy(go => go.item.baseValue).ToList();
                    break;
                }
        }

        PopulatePanel(newList);
    }

    protected virtual void DisplayApparelCategory(Inventory inventory)
    {
        activePanel = 1;

        itemInfoText1.text = "TYPE";
        itemInfoText2.text = "ARMOR";
        itemInfoText3.text = "WEIGHT";
        itemInfoText4.text = "VALUE";
        info_1_Button.interactable = true;

        SortList(inventory.apparel);
        PopulatePanel(inventory.apparel);
    }

    protected virtual void DisplayWeaponsCategory(Inventory inventory)
    {
        activePanel = 2;

        itemInfoText1.text = "TYPE";
        itemInfoText2.text = "DAMAGE";
        itemInfoText3.text = "WEIGHT";
        itemInfoText4.text = "VALUE";
        info_1_Button.interactable = true;

        SortList(inventory.weapons);
        PopulatePanel(inventory.weapons);
    }

    protected virtual void DisplayPotionsCategory(Inventory inventory)
    {
        activePanel = 3;

        itemInfoText1.text = "TYPE";
        itemInfoText2.text = "DUR.";
        itemInfoText3.text = "WEIGHT";
        itemInfoText4.text = "VALUE";
        info_1_Button.interactable = true;

        SortList(inventory.potions);
        PopulatePanel(inventory.potions);
    }

    /*protected virtual void DisplayCraftingCategory(Inventory inventory)
    {
        activePanel = 4;

        itemInfoText1.text = "";
        itemInfoText2.text = "TYPE";
        itemInfoText3.text = "WEIGHT";
        itemInfoText4.text = "VALUE";
        info_1_Button.interactable = false;

        //SortList(inventory.crafting);
        //PopulatePanel(inventory.crafting);
    }*/

    protected virtual void DisplayMiscCategory(Inventory inventory)
    {
        activePanel = 5;

        itemInfoText1.text = "";
        itemInfoText2.text = "TYPE";
        itemInfoText3.text = "WEIGHT";
        itemInfoText4.text = "VALUE";
        info_1_Button.interactable = false;

        SortList(inventory.miscellaneous);
        PopulatePanel(inventory.miscellaneous);
    }
    #endregion

    #region - Item Info -
    protected virtual string ItemName(InventoryItem inventoryItem)
    {
        Item item = DatabaseManager.GetItem(inventoryItem.itemID);
        if (inventoryItem.quantity > 1)
        {
            return item.name + " (" + inventoryItem.quantity + ")";
        }
        else return item.name;
    }

    //The class of the item within its category
    protected virtual string ItemClass(InventoryItem inventoryItem)
    {
        Item item = DatabaseManager.GetItem(inventoryItem.itemID);

        if (item is Apparel apparel) return apparel.equipSlot.ToString();
        else if (item is Weapon weapon) return weapon.weaponClass;
        else if (item is Potion potion) return potion.potionClass.ToString();
        //else if (item is CraftingComponent ingredient) return ingredient.type.ToString(); //Alchemy, Smithing, Fabrication
        else if (item is Miscellaneous misc) return "";
        return "";
    }

    //The magnitude of the item's primary property
    protected virtual string ItemMagnitude(InventoryItem inventoryItem)
    {
        Item item = DatabaseManager.GetItem(inventoryItem.itemID);

        if (item is Apparel apparel) return apparel.armor.ToString();
        else if (item is Weapon weapon)
        {
            switch (weapon.defaultAttack)
            {
                case AttackDirection.Bash:
                    {
                        return weapon.minBashDmg + "-" + weapon.maxBashDmg;
                    }
                case AttackDirection.Slash:
                    {
                        return weapon.minSlashDmg + "-" + weapon.maxSlashDmg;
                    }
                case AttackDirection.Thrust:
                    {
                        return weapon.minThrustDmg + "-" + weapon.maxThrustDmg;
                    }
            }
        }
        else if (item is Potion potion) return potion.potionEffects[0].duration.ToString();
        //else if (item is Ingredient ingredient) return ingredient.craftingClass.ToString();
        else if (item is Miscellaneous misc) return misc.miscClass.ToString();
        return "";
    }
    #endregion

    #region - Auxiliary Functions -
    public virtual void ClearInventoryPanelItems()
    {
        foreach (Transform child in InventoryPanelItem.transform.Cast<Transform>().ToList())
        {
            ObjectPooler.ReturnToPool_Static("inventoryElement", child.gameObject);
        }
    }

    protected virtual void PopulatePanel(List<InventoryItem> list)
    {
        ClearInventoryPanelItems();

        foreach (InventoryItem inventoryItem in list)
        {
            GameObject newItem = ObjectPooler.SpawnFromPool_Static("inventoryElement", transform.position, Quaternion.identity);
            InventoryItemUI txtItem = newItem.GetComponent<InventoryItemUI>();
            Item item = DatabaseManager.GetItem(inventoryItem.itemID);
            ButtonSettings(txtItem, inventoryItem);
            txtItem.itemName.text = ItemName(inventoryItem);
            txtItem.itemInfo1.text = ItemClass(inventoryItem);
            txtItem.itemInfo2.text = ItemMagnitude(inventoryItem);
            txtItem.itemInfo3.text = (item.weight * inventoryItem.quantity).ToString();
            txtItem.itemInfo4.text = (Mathf.RoundToInt(item.baseValue * costModifier)).ToString();
            switch (item.currencyType)
            {
                case CurrencyType.Copper:
                    {
                        txtItem.itemInfo4.text += "c";
                        break;
                    }
                case CurrencyType.Silver:
                    {
                        txtItem.itemInfo4.text += "s";
                        break;
                    }
                case CurrencyType.Gold:
                    {
                        txtItem.itemInfo4.text += "g";
                        break;
                    }
            }

            newItem.transform.SetParent(InventoryPanelItem);
            newItem.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    protected virtual void ButtonSettings(InventoryItemUI txtItem, InventoryItem inventoryItem)
    {
        //Meant to be overwritten
    }
    #endregion

    #region - Interaction Functions -
    //Transfers items between inventories
    protected virtual void TransferItems(Item item, int count, Inventory otherInventory, bool addItem)
    {
        if (addItem == true)
        {
            inventory.AddItem(item, count);
            otherInventory.RemoveItem(item, count);
        }
        else
        {
            inventory.RemoveItem(item, count);
            otherInventory.AddItem(item, count);
        }
    }

    //Used to move items between player inventory and storage
    protected virtual void TransferStorage(InventoryItem inventoryItem, Inventory otherInventory)
    {
        Item item = DatabaseManager.GetItem(inventoryItem.itemID);
        if (inventoryItem.quantity < 5)
        {
            TransferItems(item, 1, otherInventory, false);
            //TransferItems(inventoryItem.item, 1, otherInventory, false);
        }
        else
        {
            quantitySelect.DisplayPanel(inventoryItem.quantity);
            quantitySelect.confirmButton.onClick.AddListener(delegate
            {
                TransferItems(item, (int)quantitySelect.slider.value, otherInventory, false);
                //TransferItems(inventoryItem.item, (int)quantitySelect.slider.value, otherInventory, false);
                quantitySelect.gameObject.SetActive(false);
            });
            quantitySelect.cancelButton.onClick.AddListener(delegate
            {
                quantitySelect.gameObject.SetActive(false);
                return;
            });
        }
    }

    protected virtual void SellItem(InventoryItem inventoryItem, Inventory otherInventory)
    {
        Item item = DatabaseManager.GetItem(inventoryItem.itemID);
        if (otherInventory.TotalCoinageHoldings() >= inventory.CommonCoinageValue(item.currencyType, item.baseValue))
        {
            if (inventoryItem.quantity < 5)
            {
                otherInventory.RemoveCurrency(item.currencyType, Mathf.RoundToInt(item.baseValue * costModifier));
                inventory.AddCurrency(item.currencyType, Mathf.RoundToInt(item.baseValue * costModifier));

                TransferItems(item, 1, otherInventory, false);
            }
            else
            {
                int maxQuantity = inventoryItem.quantity;
                int price = inventory.CommonCoinageValue(item.currencyType, Mathf.RoundToInt(item.baseValue * costModifier));
                if (price >= 1)
                {
                    maxQuantity = (otherInventory.TotalCoinageHoldings() / price);
                    maxQuantity = Mathf.Clamp(maxQuantity, 0, inventoryItem.quantity);
                }

                quantitySelect.DisplayPanel(maxQuantity);
                quantitySelect.confirmButton.onClick.AddListener(delegate
                {
                    otherInventory.RemoveCurrency(item.currencyType, Mathf.RoundToInt(item.baseValue * costModifier) * (int)quantitySelect.slider.value);
                    inventory.AddCurrency(item.currencyType, Mathf.RoundToInt(item.baseValue * costModifier) * (int)quantitySelect.slider.value);
                    
                    TransferItems(item, (int)quantitySelect.slider.value, otherInventory, false);

                    quantitySelect.gameObject.SetActive(false);
                });
                quantitySelect.cancelButton.onClick.AddListener(delegate
                {
                    quantitySelect.gameObject.SetActive(false);
                    return;
                });
            }
        }
        else
        {
            UIManager.instance.DisplayPopup("Not Enough Coin");
        }
    }
    #endregion
}

public enum InventoryInteraction { Player, Looting, Storage, Bartering }