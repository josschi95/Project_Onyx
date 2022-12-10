using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : MonoBehaviour, IInteractable
{
    public Transform craftingPosition;
    [HideInInspector] public CharacterController currentOccupant;
    public CraftingType craftingType;

    public float interactionDistance = 2;

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        if (currentOccupant != null) return false;
        return true;
    }

    public void Interact(CharacterController controller)
    {
        if (currentOccupant != null)
        {
            if (currentOccupant is not PlayerController)
            {
                UIManager.instance.DisplayPopup("Somebody else is using this");
                currentOccupant.isInteracting = false;
            }
            return;
        }
        else
        {
            currentOccupant = controller;
            StartCoroutine(controller.Crafting(this, craftingType));
        }
    }

    public void ClearOccupant()
    {
        if (currentOccupant != null && currentOccupant.isInteracting == true)
            currentOccupant.isInteracting = false;

        currentOccupant = null;
    }

    public bool interacting;
    public string interactionMethod = "Sit";

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, transform.name, false);
    }
}

public enum CraftingType { alchemy, fabrication, smithing }
