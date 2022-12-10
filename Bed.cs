using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    [HideInInspector] private CharacterController currentOccupant;

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

    public string interactionMethod = "Sleep";

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, transform.name, false);
    }

    public void Interact(CharacterController controller)
    {
        if (currentOccupant != null)
        {
            //if it's an NPC, this should kick them out of the spot
            if (currentOccupant is not PlayerController)
            {
                UIManager.instance.DisplayPopup("Somebody else is using this");
                //if (currentOccupant.onInterruptCallback != null)
                    //currentOccupant.onInterruptCallback.Invoke();
                currentOccupant.isSitting = false;
            }
            return;
        }
        else
        {
            currentOccupant = controller;
            StartCoroutine(controller.Sleeping(this));
        }
    }

    public void ClearOccupant()
    {
        if (currentOccupant != null && currentOccupant.isSleeping)
            currentOccupant.isSleeping = false;

        currentOccupant = null;
    }
}
