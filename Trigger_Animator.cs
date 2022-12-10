using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Animator : Trigger
{
    [SerializeField] Animator[] anims;

    public override void OnTriggerEvent()
    {
        if (onTriggerCallback != null)
        {
            onTriggerCallback.Invoke();
        }
        TriggerAnimator();
    }

    private void TriggerAnimator()
    {
        for (int i = 0; i < anims.Length; i++)
        {
            string triggerName = anims[i].GetParameter(0).name;
            anims[i].SetTrigger(triggerName);
        }
    }
}
