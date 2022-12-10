using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPickup : MonoBehaviour, IInteractable
{
    public Spell[] spells;

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
        for (int i = 0; i < spells.Length; i++)
        {
            PlayerSpellcasting.instance.LearnSpell(spells[i]);
        }
    }

    public string interactionMethod = "Study";
    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, transform.name, false);
    }
}
