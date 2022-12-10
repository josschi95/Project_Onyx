using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : MonoBehaviour, IInteractable
{
    public Ability[] abilities;

    public float interactionDistance = 2;

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        return true;
    }

    public void Interact(CharacterController controller)
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            PlayerInventory.instance.LearnAbility(abilities[i]);
        }
    }

    public string interactionMethod = "Study";
    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, transform.name, false);
    }
}
