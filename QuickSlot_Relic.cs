using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot_Relic : QuickSlot
{
    public Item relic;

    private void Start()
    {
        OnAssignRelic(null);
    }

    public void OnAssignRelic(Item newRelic)
    {
        if (newRelic == null)
        {
            relic = null;
            slotImage.sprite = null;
            slotImage.enabled = false;
            slotName.text = "";
        }
        else
        {
            relic = newRelic;
            slotImage.enabled = true;
            slotImage.sprite = newRelic.icon;
            slotName.text = newRelic.name;
        }
    }

    public override void OnUse()
    {
        if (relic == null)
        {
            UIManager.instance.DisplayPopup("No Assigned Relic");
            return;
        }
        //relic.Use(PlayerController.instance);
    }
}
