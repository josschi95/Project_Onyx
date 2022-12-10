using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* A base class for anything that holds more than one type of item */
public class Inventory : MonoBehaviour
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int copperPieces;//{ get; protected set; }
    public int silverPieces;//{ get; protected set; }
    public int goldPieces;//{ get; protected set; }

    public List<InventoryItem> apparel = new List<InventoryItem>();
    public List<InventoryItem> weapons = new List<InventoryItem>();
    public List<InventoryItem> potions = new List<InventoryItem>();
    //public List<InventoryItem> crafting = new List<InventoryItem>();
    public List<InventoryItem> miscellaneous = new List<InventoryItem>();
    public bool isEmpty { get; protected set; }

    protected virtual void Start()
    {
        if (IsEmpty()) isEmpty = true;
        onItemChangedCallback += CheckEmpty;

        for (int i = 0; i < apparel.Count; i++)
        {
            if (apparel[i].item != null)
                apparel[i].itemID = apparel[i].item.itemID;
        }
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].item != null)
                weapons[i].itemID = weapons[i].item.itemID;
        }
        for (int i = 0; i < potions.Count; i++)
        {
            if (potions[i].item != null)
                potions[i].itemID = potions[i].item.itemID;
        }
        for (int i = 0; i < miscellaneous.Count; i++)
        {
            if (miscellaneous[i].item != null)
                miscellaneous[i].itemID = miscellaneous[i].item.itemID;
        }
    }

    #region - Items -
    //Add item to appropriate list
    public virtual void AddItem (int itemID, int quantity)
    {
        Item item = DatabaseManager.GetItem(itemID);
        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    AddMethod(item, quantity, apparel);
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    AddMethod(item, quantity, weapons);
                    break;
                }
            case ItemCategory.POTION:
                {
                    AddMethod(item, quantity, potions);
                    break;
                }
            /*case ItemCategory.CRAFTING:
                {
                    AddMethod(item, quantity, crafting);
                    break;
                }*/
            case ItemCategory.MISC:
                {
                    AddMethod(item, quantity, miscellaneous);
                    break;
                }
        }
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public virtual void AddItem(Item item, int quantity)
    {
        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    AddMethod(item, quantity, apparel);
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    AddMethod(item, quantity, weapons);
                    break;
                }
            case ItemCategory.POTION:
                {
                    AddMethod(item, quantity, potions);
                    break;
                }
            /*case ItemCategory.CRAFTING:
                {
                    AddMethod(item, quantity, crafting);
                    break;
                }*/
            case ItemCategory.MISC:
                {
                    AddMethod(item, quantity, miscellaneous);
                    break;
                }
        }
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public virtual void RemoveItem(int itemID, int quantity)
    {
        Item item = DatabaseManager.GetItem(itemID);
        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    RemoveMethod(item, quantity, apparel);
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    RemoveMethod(item, quantity, weapons);
                    break;
                }
            case ItemCategory.POTION:
                {
                    RemoveMethod(item, quantity, potions);
                    break;
                }
            /*case ItemCategory.CRAFTING:
                {
                    RemoveMethod(item, quantity, crafting);
                    break;
                }*/
            case ItemCategory.MISC:
                {
                    RemoveMethod(item, quantity, miscellaneous);
                    break;
                }
        }
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    //Remove item from appropriate list
    public virtual void RemoveItem(Item item, int quantity)
    {
        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    RemoveMethod(item, quantity, apparel);
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    RemoveMethod(item, quantity, weapons);
                    break;
                }
            case ItemCategory.POTION:
                {
                    RemoveMethod(item, quantity, potions);
                    break;
                }
            /*case ItemCategory.CRAFTING:
                {
                    RemoveMethod(item, quantity, crafting);
                    break;
                }*/
            case ItemCategory.MISC:
                {
                    RemoveMethod(item, quantity, miscellaneous);
                    break;
                }
        }
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    protected virtual void AddMethod(Item item, int quantity, List<InventoryItem> list)
    {
        if (item.isStackable)
        {
            bool itemAlreadyInInventory = false;
            foreach (InventoryItem inventoryItem in list)
            {
                if (inventoryItem.itemID == item.itemID)
                {
                    inventoryItem.quantity += quantity;
                    itemAlreadyInInventory = true;
                }
            }
            if (!itemAlreadyInInventory)
            {
                if (list == weapons || list == apparel)
                {
                    list.Add(new InventoryEquipment(item.itemID, quantity));
                }
                else list.Add(new InventoryItem(item.itemID, quantity));
            }
        }
        else
        {
            for (int i = 0; i < quantity; i++)
            {
                if (list == weapons || list == apparel)
                {
                    list.Add(new InventoryEquipment(item.itemID, 1));
                }
                else list.Add(new InventoryItem(item.itemID, 1));
            }
        }
    }

    protected virtual void RemoveMethod(Item item, int quantity, List<InventoryItem> list)
    {
        InventoryItem itemToRemove = null;

        foreach (InventoryItem inventoryItem in list)
        {
            if (inventoryItem.itemID == item.itemID)
            {
                if (inventoryItem.quantity > 1) //it's stackable
                {
                    if (inventoryItem.quantity > quantity)
                    {
                        inventoryItem.quantity -= quantity;
                    }
                    else itemToRemove = inventoryItem;
                }
                else itemToRemove = inventoryItem;
            }
        }

        if (itemToRemove != null) 
            RemoveFromList(itemToRemove, list);
    }

    //This mainly exists for the CharacterInventory class
    protected virtual void RemoveFromList(InventoryItem item, List<InventoryItem> list)
    {
        list.Remove(item);
    }

    //Used to check if there is an equal or greater number of items in the inventory 
    public virtual bool QueryInventoryContents(int itemID, int quantity)
    {
        List<InventoryItem> list = null;
        Item item = DatabaseManager.GetItem(itemID);

        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    list = apparel;
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    list = weapons;
                    break;
                }
            case ItemCategory.POTION:
                {
                    list = potions;
                    break;
                }
            /*case ItemCategory.CRAFTING:
                {
                    list = crafting;
                    break;
                }*/
            case ItemCategory.MISC:
                {
                    list = miscellaneous;
                    break;
                }
        }

        foreach (InventoryItem inventoryItem in list)
        {
            if (inventoryItem.itemID == item.itemID && inventoryItem.quantity >= quantity)
            {
                return true;
            }
        }
        return false;
    }

    /*public virtual bool QueryInventoryContents(Item item, int quantity)
    {
        List<InventoryItem> list = null;

        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    list = apparel;
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    list = weapons;
                    break;
                }
            case ItemCategory.POTION:
                {
                    list = potions;
                    break;
                }
            case ItemCategory.CRAFTING:
                {
                    list = crafting;
                    break;
                }
            case ItemCategory.MISC:
                {
                    list = miscellaneous;
                    break;
                }
        }

        foreach (InventoryItem inventoryItem in list)
        {
            //if (inventoryItem.item == item && inventoryItem.quantity >= quantity)
            {
                return true;
            }
        }
        return false;
    }*/

    //Returns the quantity of an item in the inventory
    public virtual int QueryItemCount(int itemID)
    {
        List<InventoryItem> list = null;
        Item item = DatabaseManager.GetItem(itemID);

        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    list = apparel;
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    list = weapons;
                    break;
                }
            case ItemCategory.POTION:
                {
                    list = potions;
                    break;
                }
            /*case ItemCategory.CRAFTING:
                {
                    list = crafting;
                    break;
                }*/
            case ItemCategory.MISC:
                {
                    list = miscellaneous;
                    break;
                }
        }

        foreach (InventoryItem inventoryItem in list)
        {
            if (inventoryItem.itemID == item.itemID)
            {
                if (inventoryItem.quantity <= 0)
                {
                    RemoveFromList(inventoryItem, list);
                    return 0;
                }
                return inventoryItem.quantity;
            }
        }
        return 0;
    }

    /*public virtual int QueryItemCount(Item item)
    {
        List<InventoryItem> list = null;

        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    list = apparel;
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    list = weapons;
                    break;
                }
            case ItemCategory.POTION:
                {
                    list = potions;
                    break;
                }
            case ItemCategory.CRAFTING:
                {
                    list = crafting;
                    break;
                }
            case ItemCategory.MISC:
                {
                    list = miscellaneous;
                    break;
                }
        }

        foreach (InventoryItem inventoryItem in list)
        {
            //if (inventoryItem.item == item)
            {
                if (inventoryItem.quantity <= 0)
                {
                    RemoveFromList(inventoryItem, list);
                    return 0;
                }
                return inventoryItem.quantity;
            }
        }
        return 0;
    }*/

    public virtual void ClearInventory()
    {
        apparel.Clear();
        weapons.Clear();
        potions.Clear();
        //crafting.Clear();
        miscellaneous.Clear();
        copperPieces = 0;
        silverPieces = 0;
        goldPieces = 0;

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
    #endregion

    #region - Currency -
    public virtual void AddCurrency(CurrencyType type, int amount)
    {
        if (type == CurrencyType.Copper) copperPieces += amount;
        else if (type == CurrencyType.Silver) silverPieces += amount;
        else if (type == CurrencyType.Gold) goldPieces += amount;

        if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
    }

    public virtual void RemoveCurrency(CurrencyType type, int amount)
    {
        //Run through the simple math first, if there is enough coin in a stack to pay the amount, just pay it
        if (type == CurrencyType.Copper && copperPieces >= amount) copperPieces -= amount;
        else if (type == CurrencyType.Silver && silverPieces >= amount) silverPieces -= amount;
        else if (type == CurrencyType.Gold && goldPieces >= amount) goldPieces -= amount;
        else
        {
            int commonValue = CommonCoinageValue(type, amount);

            if (commonValue >= 100 && goldPieces > 0) commonValue = SpendGold(commonValue);
            Debug.Log("remaining value after spending gold: " + commonValue);

            if (commonValue >= 10 && silverPieces > 0) commonValue = SpendSilver(commonValue);
            Debug.Log("remaining value after spending silver: " + commonValue);

            if (commonValue > 0 && copperPieces > 0) commonValue = SpendCopper(commonValue);
            Debug.Log("remaining value after spending copper: " + commonValue);

            if (commonValue > 0) StartCoroutine(CoverRemainingCosts(commonValue));
        }

        Debug.Log(transform.name + " removed " + amount + " " + type.ToString());
        if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
    }

    protected virtual int SpendGold(int commonValue)
    {
        //if (commonValue >= 100 && goldPieces > 0) { }
        int i = Mathf.FloorToInt(commonValue * 0.01f);
        if (i > goldPieces) i = goldPieces;

        commonValue -= i * 100;
        goldPieces -= i;
        Debug.Log("Spent " + i + "g");
        return commonValue;
    }

    protected virtual int SpendSilver(int commonValue)
    {
        //if (commonValue >= 10 && silverPieces > 0) { }
        int i = Mathf.FloorToInt(commonValue * 0.1f);
        if (i > silverPieces) i = silverPieces;

        commonValue -= i * 10;
        silverPieces -= i;
        Debug.Log("Spent " + i + "s");
        return commonValue;
    }

    protected virtual int SpendCopper(int commonValue)
    {
        int i = commonValue;
        if (i > copperPieces) i = copperPieces;

        commonValue -= i;
        copperPieces -= i;
        Debug.Log("Spent " + i + "c");
        return commonValue;
    }

    protected IEnumerator CoverRemainingCosts(int commonValue)
    {
        Debug.Log("Starting Unnecessary Coroutine");
        if (copperPieces > 0) Debug.LogWarning("Something got fucked up, should not have copper going into this coroutine");

        while (commonValue > 0)
        {
            //Convert 1 gold into 10 silver
            if (goldPieces > 0)
            {
                goldPieces--;
                silverPieces += 10;
                Debug.Log("Converted 1 gold into 10 silver");
            }

            //If the value is greater than 10, just spend silver as I produce it
            if (commonValue >= 10) commonValue = SpendSilver(commonValue);
            
            //Convert 1 silver into 10 copper
            if (silverPieces > 0)
            {
                silverPieces--;
                copperPieces += 10;
                Debug.Log("Converted 1 silver into 10 copper");
            }

            //By this point I should have converted some larger coins into copper
            commonValue = SpendCopper(commonValue);
            yield return null;
        }

        if (commonValue > 0) Debug.LogWarning("Something is really fucked up, should not be spending more than is in inventory");
        if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
    }

    public virtual int CommonCoinageValue(CurrencyType type, int amount)
    {
        if (type == CurrencyType.Copper) return amount;
        else if (type == CurrencyType.Silver) return amount * 10;
        else return amount * 100;
    }

    public virtual int TotalCoinageHoldings()
    {
        //returns a value in copper pieces
        return copperPieces + (silverPieces * 10) + (goldPieces * 100);
    }
    #endregion

    protected virtual void CheckEmpty()
    {
        if (IsEmpty()) isEmpty = true;
        else isEmpty = false;
    }

    protected virtual bool IsEmpty()
    {
        isEmpty = false;
        if (apparel.Count > 0) return false;
        if (weapons.Count > 0) return false;
        if (potions.Count > 0) return false;
        //if (crafting.Count > 0) return false;
        if (miscellaneous.Count > 0) return false;
        if (goldPieces != 0 || silverPieces != 0 || copperPieces != 0) return false;

        isEmpty = true;
        return true;
    }
}
public enum CurrencyType { Copper, Silver, Gold }

[System.Serializable]
public class InventoryItem
{
    public Item item;
    public int itemID;
    public int quantity = 1;
    public bool isEquipped;
    public bool isJunk = false;
    public bool forStorage = false;

    public InventoryItem(int itemID, int quantity, bool isEquipped = false, bool isJunk = false)
    {
        //this.item = item;
        this.itemID = itemID;
        this.quantity = quantity;
        this.isJunk = isJunk;
        this.isEquipped = isEquipped;
    }

    public virtual void Use(CharacterController controller)
    {
        //Debug.Log("Using " + item.name);
        Item item = DatabaseManager.GetItem(itemID);
        if (item is Equipment)
        //if (item is Equipment)
        {
            Debug.LogWarning("ERROR: This Should be an InventoryEquipment");
        }
        else if (item is Potion potion)
        //else if (item is Potion potion)
        {
            if (controller is PlayerController)
            {
                if (PlayerStats.instance.OnUsePotion(potion))
                {
                    quantity--;
                    if (quantity <= 0) RemoveFromInventory(controller);
                }
            }
        }
    }

    public virtual void UseSecondary(CharacterController controller)
    {
        //Debug.Log("Using " + item.name);
        Item item = DatabaseManager.GetItem(itemID);
        if (item is Equipment)
        //if (item is Equipment)
        {
            Debug.LogWarning("ERROR: This Should be an InventoryEquipment");
        }
        else if (item is Potion potion)
        //else if (item is Potion potion)
        {
            if (controller is PlayerController player)
            {
                isEquipped = true;
                HUDManager.instance.quickslotManager.OnPotionsChange(potion);
                if (player.inventory.onItemChangedCallback != null)
                    player.inventory.onItemChangedCallback.Invoke();
            }
        }
    }

    public void UnequipItem(CharacterController controller)
    {
        var equip = controller.equipmentManager;
        for (int i = 0; i < equip.currentEquipment.Length; i++)
        {
            if (equip.currentEquipment[i] == this)
            {
                equip.Unequip(i);
                break;
            }
        }
        isEquipped = false;
    }

    public void RemoveFromInventory(CharacterController controller)
    {
        controller.inventory.RemoveItem(itemID, quantity);
        //controller.inventory.RemoveItem(item, quantity);
    }
}

[System.Serializable]
public class InventoryEquipment : InventoryItem
{
    public EquipmentSlot equipSlot;
    public int runeSlots;
    public List<Rune> equipmentRunes = new List<Rune>();

    public InventoryEquipment(int itemID, int quantity, bool isEquipped = false, bool isJunk = false) : base (itemID, quantity, isEquipped, isJunk)
    {
        //this.item = equipment;
        this.itemID = itemID;
        this.quantity = quantity;
        this.isJunk = isJunk;
        this.isEquipped = isEquipped;
        Equipment equip = DatabaseManager.GetItem(itemID) as Equipment;
        equipSlot = equip.equipSlot;
    }

    public override void Use(CharacterController controller)
    {
        if (isEquipped) UnequipItem(controller);

        var equip = DatabaseManager.GetItem(itemID) as Equipment;
        //var equip = item as Equipment;
        int slotIndex = (int)equip.equipSlot;

        if (equip is Weapon weapon)
        {
            if (controller.equipmentManager.usingSecondaryWeaponSet) slotIndex += 2;
        }

        isEquipped = true;
        controller.equipmentManager.Equip(this, slotIndex);
    }

    public override void UseSecondary(CharacterController controller)
    {
        if (isEquipped) UnequipItem(controller);

        var equip = DatabaseManager.GetItem(itemID) as Equipment;
        //var equip = item as Equipment;
        int slotIndex = (int)equip.equipSlot;

        if (equip is Weapon weapon)
        {
            //One-Handed and Thrown weapons can be equipped in left hand
            if (weapon.weaponType == WeaponType.One_Handed || weapon.weaponType == WeaponType.Thrown) slotIndex++;

            if (controller.equipmentManager.usingSecondaryWeaponSet) slotIndex += 2;
        }
        else if (equip.equipSlot == EquipmentSlot.Ring_Left) slotIndex++;

        isEquipped = true;
        controller.equipmentManager.Equip(this, slotIndex);
    }

    #region - Runes -
    public bool CanInsertRune()
    {
        if (equipmentRunes.Count >= runeSlots) return false;
        return true;
    }

    public void InsertRune(Rune rune)
    {
        if (CanInsertRune() == false) return;
        equipmentRunes.Add(rune);
    }

    public void RemoveRune(Rune rune)
    {
        if (equipmentRunes.Contains(rune))
            equipmentRunes.Remove(rune);
    }
    #endregion
}