using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;

public class PlayerInventoryDisplay : InventoryDisplay
{
    [SerializeField] private Button  abilitiesButton, spellsButton;
    public TMP_Text carryCapacityText, coinageText;
    [HideInInspector] public Inventory otherInventory;

    private PlayerInventory playerInventory;
    private PlayerEquipmentManager playerEquipment;
    private PlayerStats playerStats;
    private PlayerController playerController;
    private PlayerSpellcasting spellcasting;

    protected override void Start()
    {
        playerEquipment = PlayerEquipmentManager.instance;
        playerController = PlayerController.instance;
        playerInventory = PlayerInventory.instance;
        spellcasting = PlayerSpellcasting.instance;
        inventory = playerInventory;
        playerStats = PlayerStats.instance;
        base.Start();
        abilitiesButton.onClick.AddListener(() => { DisplayAbilityCategory(); });
        spellsButton.onClick.AddListener(() => { DisplaySpellCategory(); });
    }

    public void OnMenuOpen()
    {
        playerInventory.onItemChangedCallback += UpdatePanel;
        playerEquipment.onEquipmentChanged += delegate { UpdatePanel(); };
    }

    public void OnMenuClose()
    {
        playerInventory.onItemChangedCallback -= UpdatePanel;
        playerEquipment.onEquipmentChanged -= delegate { UpdatePanel(); };
        ClearInventoryPanelItems();
    }

    public override void UpdatePanel()
    {
        carryCapacityText.text = "Carry Capacity: " + playerInventory.netCarryWeight + "/" + playerStats.statSheet.carryCapacity.GetValue().ToString();
        coinageText.text = playerInventory.copperPieces + "c, " + playerInventory.silverPieces + "s, " + playerInventory.goldPieces + "g";
        if (activePanel == 6) DisplayAbilityCategory();
        else if (activePanel == 7) DisplaySpellCategory();
        else base.UpdatePanel();
    }

    private void DisplayAbilityCategory()
    {
        activePanel = 6;

        itemInfoText1.text = "";
        itemInfoText2.text = "TYPE";
        itemInfoText3.text = "DUR.";
        itemInfoText4.text = "COOLDOWN";
        info_1_Button.interactable = false;

        SortAbilities();
        PopulateAbilityPanel();
    }

    private void DisplaySpellCategory()
    {
        activePanel = 7;

        itemInfoText1.text = "RANGE";
        itemInfoText2.text = "COST";
        itemInfoText3.text = "DOM.";
        itemInfoText4.text = "SCHOOL";
        info_1_Button.interactable = true;

        SortSpells();
        PopulateSpellPanel();
    }

    private void SortAbilities()
    {
        List<Ability> list = playerInventory.abilities;

        switch (sortOrder)
        {
            case 0:
                {
                    list.OrderBy(x => x.name).ToList();
                    break;
                }
            case 1:
                {
                    list.OrderBy(x => x.type).ToList();
                    break;
                }
            case 2:
                {
                    list.OrderBy(x => x.duration).ToList();
                    break;
                }
            case 3:
                {
                    list.OrderBy(x => x.cooldownDuration).ToList();
                    break;
                }
        }
    }

    private void SortSpells()
    {
        List<Spell> list = spellcasting.arcaneSpellbook;

        switch (sortOrder)
        {
            case 0:
                {
                    list.OrderBy(x => x.name).ToList();
                    break;
                }
            case 1:
                {
                    list.OrderBy(x => x.spellEffects[0].effectRange).ToList();
                    break;
                }
            case 2:
                {
                    list.OrderBy(x => x.manaCost).ToList();
                    break;
                }
            case 3:
                {
                    list.OrderBy(x => x.spellDomain).ToList();
                    break;
                }
            case 4:
                {
                    list.OrderBy(x => x.favoredSchool).ToList();
                    break;
                }
        }
    }

    private void PopulateAbilityPanel()
    {
        ClearInventoryPanelItems();

        foreach (Ability ability in playerInventory.abilities)
        {
            GameObject newItem = ObjectPooler.SpawnFromPool_Static("inventoryElement", transform.position, Quaternion.identity);
            InventoryItemUI txtItem = newItem.GetComponent<InventoryItemUI>();

            txtItem.ability = ability;
            txtItem.itemImage.sprite = ability.icon;
            txtItem.itemName.text = (ability.name).ToString();

            //Button Clicks
            RightClick newInput = txtItem.itemButton.GetComponent<Button>().GetComponent<RightClick>();
            newInput.leftClick.AddListener(ability.ReadyAbility);
            newInput.rightClick.AddListener(ability.Prepare);
            if (HUDManager.instance.quickslotManager.preparedAbilities.Contains(ability))
                txtItem.equippedFrame.enabled = true;
            else
                txtItem.equippedFrame.enabled = false;

            txtItem.itemInfo2.text = ability.type.ToString();
            txtItem.itemInfo3.text = ability.duration.ToString();
            txtItem.itemInfo4.text = ability.cooldownDuration.ToString();

            newItem.transform.SetParent(InventoryPanelItem);
            newItem.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void PopulateSpellPanel()
    {
        ClearInventoryPanelItems();

        foreach (Spell spell in spellcasting.arcaneSpellbook)
        {
            GameObject newItem = ObjectPooler.SpawnFromPool_Static("inventoryElement", transform.position, Quaternion.identity);
            InventoryItemUI txtItem = newItem.GetComponent<InventoryItemUI>();

            txtItem.spell = spell;
            txtItem.itemImage.sprite = spell.icon;
            txtItem.itemName.text = (spell.name).ToString();

            //Button Clicks
            RightClick newInput = txtItem.itemButton.GetComponent<Button>().GetComponent<RightClick>();
            newInput.leftClick.AddListener(spell.ReadySpell);
            newInput.rightClick.AddListener(spell.Prepare);
            if (HUDManager.instance.quickslotManager.preparedSpells.Contains(spell))
                txtItem.equippedFrame.enabled = true;
            else
                txtItem.equippedFrame.enabled = false;

            txtItem.itemInfo1.text = spell.spellEffects[0].effectRange.ToString();
            txtItem.itemInfo2.text = spell.manaCost.ToString();
            txtItem.itemInfo3.text = spell.spellDomain.ToString();
            txtItem.itemInfo4.text = spell.favoredSchool.ToString();

            newItem.transform.SetParent(InventoryPanelItem);
            newItem.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    protected override void ButtonSettings(InventoryItemUI txtItem, InventoryItem inventoryItem)
    {
        Item item = DatabaseManager.GetItem(inventoryItem.itemID);
        txtItem.item = item;
        //txtItem.item = inventoryItem.item;
        txtItem.spell = null;
        txtItem.itemImage.sprite = item.icon;
        //txtItem.itemImage.sprite = inventoryItem.item.icon;
        txtItem.equippedFrame.enabled = inventoryItem.isEquipped;

        if (inventoryInteraction == InventoryInteraction.Player)
        {
            SetButtonToEquip(txtItem, inventoryItem);
        }
        else if (inventoryInteraction == InventoryInteraction.Storage)
        {
            SetButtonToTransfer(txtItem, inventoryItem);
        }
        else if (inventoryInteraction == InventoryInteraction.Bartering)
        {
            SetButtonToSell(txtItem, inventoryItem);
        }
        else Debug.Log("ERROR");
    }

    private void SetButtonToEquip(InventoryItemUI txtItem, InventoryItem inventoryItem)
    {
        RightClick newInput = txtItem.rightClick;
        newInput.middleClick.AddListener(delegate { DropItem(inventoryItem); });

        //I may (in the future, not now) add another reference onto the Use/Unequip whatever thing
        //if I implement restrictions on equipping/unequipping items (curses, stat requirements, etc.)
        //This should work for now, so I'll leave it be

        //Item is equipped
        if (inventoryItem is InventoryEquipment equip && equip.isEquipped)
        {
            //Item is a weapon
            if (DatabaseManager.GetItem(equip.itemID) is Weapon newWeapon)
            {
                //Item can be equipped in either hand
                if (newWeapon.weaponType == WeaponType.One_Handed || newWeapon.weaponType == WeaponType.Thrown)
                {
                    if (equip.equipSlot == EquipmentSlot.Primary_Main || equip.equipSlot == EquipmentSlot.Secondary_Main)
                    {
                        newInput.leftClick.AddListener(delegate { inventoryItem.UnequipItem(playerController); });
                        newInput.rightClick.AddListener(delegate { inventoryItem.UseSecondary(playerController); });
                    }
                    else if (equip.equipSlot == EquipmentSlot.Primary_Off || equip.equipSlot == EquipmentSlot.Secondary_Off)
                    {
                        newInput.leftClick.AddListener(delegate { inventoryItem.Use(playerController); });
                        newInput.rightClick.AddListener(delegate { inventoryItem.UnequipItem(playerController); });
                    }
                }
                //Weapon is equipped but cannot swap slots
                else
                {
                    newInput.leftClick.AddListener(delegate { inventoryItem.UnequipItem(playerController); });
                    newInput.rightClick.AddListener(delegate { inventoryItem.UnequipItem(playerController); });
                }
            }
            //Item is apparel and is equipped
            else
            {
                newInput.leftClick.AddListener(delegate { inventoryItem.UnequipItem(playerController); });
                newInput.rightClick.AddListener(delegate { inventoryItem.UnequipItem(playerController); });
            }
        }
        //Item is either not equipped or not equipment
        else
        {
            newInput.leftClick.AddListener(delegate { inventoryItem.Use(playerController); });
            newInput.rightClick.AddListener(delegate { inventoryItem.UseSecondary(playerController); });
        }
    }

    private void DropItem(InventoryItem inventoryItem)
    {
        //If greater than 1, pullup quantity selection panel
        //Remove number from inventory
        //Spawn dropped item bag

        Debug.Log("Dropping " + DatabaseManager.GetItem(inventoryItem.itemID).name);
    }

    private void SetButtonToTransfer(InventoryItemUI txtItem, InventoryItem inventoryItem)
    {
        if (otherInventory == null)
        {
            Debug.Log("other inventory null");
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