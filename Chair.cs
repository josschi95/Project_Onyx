using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour, IInteractable, INPCInteractNode
{
    public Transform seatedPosition;
    public Transform departurePosition;
    private CharacterController currentOccupant;

    public bool interacting;
    public float interactionDistance = 2;
    public string interactionMethod = "Sit";

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
            //if it's an NPC, this should kick them out of the spot
            if (currentOccupant is not PlayerController)
            {
                UIManager.instance.DisplayPopup("Somebody else is using this");
                currentOccupant.isSitting = false;
            }
            return;
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
            currentOccupant = controller;
            StartCoroutine(controller.Sitting(this));
        }
    }



    public void ClearOccupant()
    {
        if (currentOccupant != null && currentOccupant.isSitting)
            currentOccupant.isSitting = false;

        currentOccupant = null;
        GetComponent<Collider>().isTrigger = false;
    }
    
    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, transform.name, false);
    }
}
