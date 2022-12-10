using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItemUI : MonoBehaviour, IPooledObject, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public Spell spell;
    public Ability ability;

    [Space]
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemInfo1;
    public TMP_Text itemInfo2;
    public TMP_Text itemInfo3;
    public TMP_Text itemInfo4;
    public Button itemButton;
    public Image equippedFrame;
    [Space]
    public RightClick rightClick;

    public void OnObjectSpawn()
    {
        itemButton.onClick.RemoveAllListeners();
        rightClick.rightClick.RemoveAllListeners();
        rightClick.middleClick.RemoveAllListeners();
        rightClick.leftClick.RemoveAllListeners();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            Tooltip_Item.ShowTooltipItem_Static(item);
        }
        else if (spell != null)
        {
            Tooltip_Item.ShowTooltipSpell_Static(spell);
        }
        else if (ability != null)
        {
            Tooltip_Item.ShowTooltipAbility_Static(ability);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip_Item.HideTooltip_Static();
    }
}
