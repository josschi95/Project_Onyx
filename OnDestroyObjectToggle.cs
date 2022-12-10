using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyObjectToggle : MonoBehaviour
{
    [SerializeField] Destructable destroyedObject;
    [SerializeField] GameObject[] objectsToToggle;
    [Tooltip("Is the ObjectToToggle turned on or off. False = off")]
    public bool toggleOn = false;

    // Start is called before the first frame update
    void Start()
    {
        destroyedObject.onDestroyCallback += TriggerObjectToggle;
    }

    private void TriggerObjectToggle()
    {
        if (toggleOn == true)
        {
            for (int i = 0; i < objectsToToggle.Length; i++)
            {
                objectsToToggle[i].SetActive(true);
            }
            
        }
        else
        {
            for (int i = 0; i < objectsToToggle.Length; i++)
            {
                objectsToToggle[i].SetActive(false);
            }
            
        }
    }
}
