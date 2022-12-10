using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableAnimator : MonoBehaviour, IInteractable
{
    public string interactionDisplay;
    public string interactName;
    [SerializeField] private Animation[] anim;
    [SerializeField] private Outline outline;

    public float interactionDistance = 2;

    
    public bool CanBeAccessed(float distance)
    {
        return true;
    }

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionDisplay, interactName, false);
    }

    public void Interact(CharacterController controller)
    {
        for (int i = 0; i < anim.Length; i++)
        {
            anim[i].Play();
        }
    }
}