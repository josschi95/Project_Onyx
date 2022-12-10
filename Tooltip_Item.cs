using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip_Item : MonoBehaviour
{
    #region - Singleton -
    private static Tooltip_Item instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    private PlayerEquipmentManager playerEquipmentManager;
    private bool isActive = false;
    [SerializeField] private RectTransform tip;
    [SerializeField] private RectTransform backgroundRectTransform;
    [SerializeField] private RectTransform canvasRectTransform;

    [Header("Item Information")]
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemClass;
    [SerializeField] private TMP_Text itemMagnitude;
    [SerializeField] private TMP_Text itemDetails;

    [SerializeField] private TMP_Text itemFlavor;
    [SerializeField] private TMP_Text itemValue;
    [SerializeField] private TMP_Text itemWeight;
    [SerializeField] private RectTransform detailsTextElement;
    [SerializeField] private RectTransform flavorTextElement;

    [SerializeField] private float tooltipWidth = 420;

    private void Start()
    {
        playerEquipmentManager = PlayerEquipmentManager.instance;
        PlayerInventory.instance.onItemChangedCallback += HideItemTooltip;
        gameObject.SetActive(false);
    }

    #region - Item Tooltip -
    private void ShowItemTooltip(Item inventoryItem)
    {
        if (InputHandler.AlternateClick_Static()) return;
        isActive = true;
        SetItemText(inventoryItem);
        gameObject.SetActive(true);
        StartCoroutine(MaintainTooltip());
    }

    private void SetItemText(Item item)
    {
        itemName.text = item.name;
        if (item.IsEquipped(playerEquipmentManager)) itemName.text += " (equipped)";
        itemIcon.sprite = item.icon;
        itemClass.text = ItemClass(item);
        itemMagnitude.text = ItemMagnitude(item);
        itemDetails.text = ItemDetails(item);

        itemFlavor.text = item.flavorText;
        
        itemValue.text = "Value: " + item.baseValue;
        itemWeight.text = "Weight: " + item.weight;

        detailsTextElement.sizeDelta = new Vector2(400, itemDetails.preferredHeight);
        flavorTextElement.sizeDelta = new Vector2(400, itemFlavor.preferredHeight);

        //padding(45) + itemName(50) + itemClass(35) + icon(150) + itemValue/weight(30)
        float height = 310 + detailsTextElement.sizeDelta.y + flavorTextElement.sizeDelta.y;
        Vector2 backgroundSize = new Vector2(tooltipWidth, height);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private string ItemClass(Item item)
    {
        if (item is Apparel apparel)
        {
            return apparel.equipSlot.ToString() + " (" + apparel.armorSkill.ToString() + ")";
        }
        else if (item is Weapon weapon)
        {

            return weapon.weaponClass + " (" + weapon.weaponSkill.ToString() + ")";
        }
        else if (item is Potion potion)
        {
            return potion.potionType.ToString() + " (" + potion.potionClass.ToString() + ")";
        }
        //else if (item is Ingredient ingredient)
        //return ingredient.craftingClass.ToString(); //Alchemy, Smithing, Runesmith

        else if (item is Miscellaneous misc) return misc.miscClass.ToString();
        return "";
    }

    private string ItemMagnitude(Item item)
    {
        if (item is Apparel apparel)
        {
            return "ARMOR\n" + apparel.armor;
        }
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
        else if (item is Potion potion)
        {
            return "";
        }
        //else if (item is Ingredient ingredient)
        //return ingredient.craftingClass.ToString(); //Alchemy, Smithing, Runesmith

        else if (item is Miscellaneous misc) return misc.miscClass.ToString();
        return "";
    }

    private string ItemDetails(Item item)
    {
        float height = 0;
        string details = "";
        switch (item)
        {
            case Apparel apparel:
                {
                    for (int i = 0; i < apparel.equipEffects.Length; i++)
                    {
                        height += 30;
                        if (i == 0) details = details + apparel.equipEffects[i].spellEffect.name + ": " + apparel.equipEffects[i].magnitude;
                        else details = details + "\n" + apparel.equipEffects[i].spellEffect.name + ": " + apparel.equipEffects[i].magnitude;
                    }
                    break;
                }
            case Weapon weapon:
                {
                    height = 90;
                    details += "BASH: " + weapon.minBashDmg + "-" + weapon.maxBashDmg + " " + weapon.bashDamage.ToString();
                    details += "\nSLASH: " + weapon.minSlashDmg + "-" + weapon.maxSlashDmg + " " + weapon.slashDamage.ToString();
                    details += "\nTHRUST: " + weapon.minThrustDmg + "-" + weapon.maxThrustDmg + " " + weapon.thrustDamage.ToString();
                    for (int i = 0; i < weapon.equipEffects.Length; i++)
                    {
                        height += 30;
                        details += "\n" + weapon.equipEffects[i].spellEffect.name + ": " + weapon.equipEffects[i].magnitude;
                    }
                    break;
                }
            case Potion potion:
                {
                    details = potion.potionType.ToString() + " (" + potion.potionClass.ToString() + ")";
                    break;
                }
            /*case CraftingComponent component:
                {
                    details = component.type.ToString();
                    break;
                }*/
            case Miscellaneous misc:
                {
                    details = misc.miscClass.ToString();
                    break;
                }
        }

        //detailsTextElement.sizeDelta = new Vector2(400, height);
        return details;
    }
    #endregion

    #region - Spell Tooltip -
    private void ShowSpellTooltip(Spell spell)
    {
        if (InputHandler.AlternateClick_Static()) return;
        isActive = true;
        SetSpellText(spell);
        gameObject.SetActive(true);
        StartCoroutine(MaintainTooltip());
    }

    private void SetSpellText(Spell spell)
    {
        itemName.text = spell.name;
        itemIcon.sprite = spell.icon;

        //itemClass.text = "Chance to cast: " + PlayerSpellcasting.instance.ChanceToCastSpell(spell).ToString();
        itemMagnitude.text = + spell.manaCost + " MANA";
        
        itemDetails.text = SpellEffects(spell);
        detailsTextElement.sizeDelta = new Vector2(400, itemDetails.preferredHeight);

        itemFlavor.text = "";
        flavorTextElement.sizeDelta = new Vector2(400, 0);

        itemValue.text = "Domain: " + spell.spellDomain.ToString();
        itemWeight.text = "School: " + spell.favoredSchool.ToString();

        //padding(45) + itemName(50) + itemClass(35) + icon(150) + itemValue/weight(30)
        float preferredHeight = 310 + detailsTextElement.sizeDelta.y;
        Vector2 backgroundSize = new Vector2(tooltipWidth, preferredHeight);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private string SpellEffects(Spell spell)
    {
        //float height = 0;
        string details = "";
        for (int i = 0; i < spell.spellEffects.Count; i++)
        {
            if (i != 0) details += "\n";
            var effect = spell.spellEffects[i];
            details += effect.EffectDescription();
            //height += 25;
        }
        //detailsTextElement.sizeDelta = new Vector2(400, height);
        return details;
    }
    #endregion

    #region - Ability Tooltip -
    private void ShowAbilityTooltip(Ability ability)
    {
        if (InputHandler.AlternateClick_Static()) return;
        isActive = true;
        SetAbilityText(ability);
        gameObject.SetActive(true);
        StartCoroutine(MaintainTooltip());
    }

    private void SetAbilityText(Ability ability)
    {
        itemName.text = ability.name;
        itemClass.text = ability.type.ToString();
        itemIcon.sprite = ability.icon;
        itemDetails.text = AbilityEffects(ability);

        itemFlavor.text = ability.flavorText;
        flavorTextElement.sizeDelta = new Vector2(400, itemFlavor.preferredHeight);
        
        itemValue.text = "Cooldown: " + ability.cooldownDuration + "s";
        if (ability.duration > 0) itemWeight.text = "Duration: " + ability.duration + "s";

        //padding(45) + itemName(50) + itemClass(35) + icon(150) + itemValue/weight(30)
        float preferredHeight = 310 + detailsTextElement.sizeDelta.y + flavorTextElement.sizeDelta.y;
        Vector2 backgroundSize = new Vector2(tooltipWidth, preferredHeight);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private string AbilityEffects(Ability ability)
    {
        float height = 0;
        string details = "";

        if (ability.userOnlyEffects.Count > 0)
        {
            details += "User Only Effects";
            height += 30;
            for (int i = 0; i < ability.userOnlyEffects.Count; i++)
            {
                string duration = "";
                if (ability.userOnlyEffects[i].duration > 0) duration = ", " + ability.userOnlyEffects[i].duration + "s";
                details = details + "\n" + ability.userOnlyEffects[i].spellEffect.name + ": " + ability.userOnlyEffects[i].magnitude + duration;
                height += 30;
            }

        }
        if (ability.positiveEffects.Count > 0)
        {
            if (height > 0) details += "\n";
            details += "All Ally Effects";
            height += 30;
            for (int i = 0; i < ability.positiveEffects.Count; i++)
            {
                string duration = "";
                if (ability.positiveEffects[i].duration > 0) duration = ", " + ability.positiveEffects[i].duration + "s";
                details = details + "\n" + ability.positiveEffects[i].spellEffect.name + ": " + ability.positiveEffects[i].magnitude + duration;
                height += 30;
            }
        }
        if (ability.negativeEffects.Count > 0)
        {
            if (height > 0) details += "\n";
            details += "Enemy Effects";
            height += 30;
            for (int i = 0; i < ability.negativeEffects.Count; i++)
            {
                string duration = "";
                if (ability.negativeEffects[i].duration > 0) duration = ", " + ability.negativeEffects[i].duration + "s";
                details = details + "\n" + ability.negativeEffects[i].spellEffect.name + ": " + ability.negativeEffects[i].magnitude + duration;
                height += 30;
            }
        }

        detailsTextElement.sizeDelta = new Vector2(400, height);
        return details;
    }

    #endregion

    #region - Static Functions -
    public static void ShowTooltipItem_Static(Item inventoryItem)
    {
        instance.ShowItemTooltip(inventoryItem);
    }

    public static void ShowTooltipSpell_Static(Spell spell)
    {
        instance.ShowSpellTooltip(spell);
    }

    public static void ShowTooltipAbility_Static(Ability ability)
    {
        instance.ShowAbilityTooltip(ability);
    }

    public static void HideTooltip_Static()
    {
        instance.HideItemTooltip();
    }
    #endregion

    private IEnumerator MaintainTooltip()
    {
        transform.SetAsLastSibling();
        while (isActive == true)
        {
            transform.position = InputHandler.GetMousePosition_Static();

            Vector2 anchoredPosition = tip.anchoredPosition;
            //If the tooltip would extend past the right side of the screen
            //The top left corner position of the box plus the width of teh box, compared to total width of the screen (left side origin)
            if (anchoredPosition.x + backgroundRectTransform.rect.width + 100 > canvasRectTransform.rect.width)
            {
                anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width - 100;
            }
            //If the tooltip would extend past the bottom of the screen
            //The top left corner position of the box minus the height of the box, compared to the total height of the screen (top side origin)
            if (anchoredPosition.y - backgroundRectTransform.rect.height - 100 < -canvasRectTransform.rect.height)
            {
                anchoredPosition.y = -canvasRectTransform.rect.height + backgroundRectTransform.rect.height + 100;
            }
            tip.anchoredPosition = anchoredPosition;

            if (InputHandler.AlternateClick_Static()) isActive = false;

            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void HideItemTooltip()
    {
        isActive = false;
    }
}
