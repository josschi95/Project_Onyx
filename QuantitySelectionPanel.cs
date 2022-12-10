using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuantitySelectionPanel : MonoBehaviour
{
    public TMP_Text quantityText;
    public Slider slider;
    public Button confirmButton;
    public Button cancelButton;

    public void DisplayPanel(int maxQuantity)
    {
        gameObject.SetActive(true);
        slider.minValue = 1;
        slider.maxValue = maxQuantity;
        slider.value = 1;
        slider.onValueChanged.AddListener(delegate { quantityText.text = slider.value.ToString(); });
    }
}
