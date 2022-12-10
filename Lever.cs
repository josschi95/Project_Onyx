using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Will likely need to make adjustments in the future to add a new callback for whether the lever has been opened or closed
   To toggle the relevant effects */

public class Lever : MonoBehaviour, IInteractable, IEffectReceiver
{
    [SerializeField] private Animator anim;
    [SerializeField] private Outline outline;
    [SerializeField] private float interactionDistance = 2;
    [SerializeField] private List<EffectHolder> trapEffects = new List<EffectHolder>();
    
    private bool acceptTraps = false;

    private void Start()
    {
        outline.enabled = false;
    }

    #region - IInteractable -
    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        return true;
    }

    public string interactionMethod = "Pull";
    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, transform.name, false);
    }

    public void Interact(CharacterController controller)
    {
        anim.SetTrigger("pull");
        if (trapEffects.Count > 0)
        {
            for (int i = 0; i < trapEffects.Count; i++)
            {
                trapEffects[i].ApplyStatEffect(controller.characterStats, transform.name);
            }
            trapEffects.Clear();
        }
    }
    #endregion

    #region - Spell Reception -
    public void TransferEffects(EffectHolder[] effects, string sourceName)
    {
        foreach (EffectHolder effect in effects)
        {
            if (effect.spellEffect is TrapObject)
            {
                acceptTraps = true;
            }
            else if (effect.spellEffect is Telekinesis)
            {
                Interact(null);
            }
            else if (acceptTraps == true)
            {
                trapEffects.Add(effect);
            }
        }
        acceptTraps = false;
    }

    public void OnDivinationEnter(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Traps && trapEffects.Count > 0)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red;
        }
    }

    public void OnDivinationExit(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Traps)
        {
            outline.enabled = false;
        }
    }
    #endregion
}