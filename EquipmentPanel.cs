using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Equipment equippedItem;
    public Image image;
    public Sprite defaultIcon;

    public void UpdatePanel(Equipment newEquip)
    {
        if (newEquip != null)
        {
            equippedItem = newEquip;
            image.sprite = newEquip.icon;
        }
        else
        {
            equippedItem = null;
            image.sprite = defaultIcon;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (equippedItem != null)
            Tooltip_Item.ShowTooltipItem_Static(equippedItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip_Item.HideTooltip_Static();
    }
}
