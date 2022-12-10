using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    #region - Singleton -
    private static Tooltip instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] private RectTransform backgroundRectTransform;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private TMP_Text tooltipText;
    private bool isActive = false;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void ShowTooltip(string text)
    {
        isActive = true;
        SetText(text);
        gameObject.SetActive(true);
        StartCoroutine(MaintainTooltip());
    }

    private void SetText(string text)
    {
        tooltipText.text = text;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void HideTooltip()
    { 
        isActive = false; 
    }

    private IEnumerator MaintainTooltip()
    {
        transform.SetAsLastSibling();

        while (isActive == true)
        {
            transform.position = InputHandler.GetMousePosition_Static();

            Vector2 anchoredPosition = transform.GetComponent<RectTransform>().anchoredPosition;
            if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
            {
                anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
            }
            if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
            {
                anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
            }
            transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

            yield return null;
        }

        gameObject.SetActive(false);

    }

    #region - Static Functions -
    public static void ShowTooltip_Static(string tooltipText)
    {
        instance.ShowTooltip(tooltipText);
    }

    public static void ShowTooltipItem_Static(Item item)
    {
        bool equipped = false;
        if (item.IsEquipped(PlayerEquipmentManager.instance))
        {
            equipped = true;
        }
        string nameText = item.name;
        if (equipped) nameText = nameText + " (equipped)";
        string newText = nameText + "\n" + "Weight: " + item.weight + "\n" + "Value: " + item.baseValue;
        instance.ShowTooltip(newText);
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
    #endregion
}
