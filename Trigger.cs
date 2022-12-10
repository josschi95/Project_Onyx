using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public delegate void OnTrigger();
    public OnTrigger onTriggerCallback;

    public virtual void OnTriggerEvent()
    {
        if (onTriggerCallback != null)
        {
            onTriggerCallback.Invoke();
        }
    }
}
