using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDetection : MonoBehaviour
{
    [SerializeField] private Collider detectionArea;
    public List<CharacterController> autoDetects = new List<CharacterController>();

    private void Start()
    {
        detectionArea.isTrigger = true;
    }

    public void ToggleDetection(bool enabled)
    {
        detectionArea.enabled = enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            CharacterController character = other.GetComponent<CharacterController>();
            if (character != null)
            {
                if (autoDetects.Contains(character) == false)
                    autoDetects.Add(character);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            CharacterController character = other.GetComponent<CharacterController>();
            if (character != null)
            {
                if (autoDetects.Contains(character))
                    autoDetects.Remove(character);
            }
        }
    }
}
