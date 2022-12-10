using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSlot : MonoBehaviour
{
    public Image background;
    public Image slotImage;
    public TMP_Text slotName;

    public virtual void OnUse()
    {
        //Meant to be overwritten
    }
}
