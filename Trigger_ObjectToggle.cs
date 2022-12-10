using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_ObjectToggle : Trigger
{
    [SerializeField] GameObject[] objectsToToggle;

    public override void OnTriggerEvent()
    {
        if (onTriggerCallback != null)
        {
            onTriggerCallback.Invoke();
        }
        ToggleObjects();
    }

    private void ToggleObjects()
    {
        for (int i = 0; i < objectsToToggle.Length; i++)
        {
            if (objectsToToggle[i].activeSelf == true)
            {
                objectsToToggle[i].SetActive(false);
            }
            else
            {
                objectsToToggle[i].SetActive(true);
            }
        }
    }
}
