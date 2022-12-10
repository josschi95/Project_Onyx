using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageContainer : Inventory, IInteractable, IEffectReceiver
{
    protected UIManager UI;

    [SerializeField] protected string containerName = "Container";
    [SerializeField] protected string interactionMethod = "Search";
    [SerializeField] protected Outline outline;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected float interactionDistance = 2;

    [Header("Traps")]
    [SerializeField] protected List<EffectHolder> trapEffects = new List<EffectHolder>();
    protected bool acceptTraps = false;

    protected override void Start()
    {
        base.Start();
        UI = UIManager.instance;
        outline.enabled = false;
    }

    protected virtual void OpenContainer()
    {
        UI.interactionInventoryDisplay.onInteractionEnd += CloseContainer;
        UI.OpenDoubleInventories(this, InventoryInteraction.Storage);
        //Play chest open AFX
    }

    public virtual void CloseContainer()
    {
        //anim.SetTrigger("rotate_x");
        UI.interactionInventoryDisplay.onInteractionEnd -= CloseContainer;
        //Play chest close AFX
    }

    #region - IInteractable -
    public virtual bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public virtual bool CanBeAccessed(float distance)
    {
        return true;
    }

    public virtual void Interact(CharacterController controller)
    {
        if (controller is PlayerController) OpenContainer();
    }

    public virtual DoubleString GetInteractionDisplay()
    {
        string name = containerName;
        if (isEmpty) name += " (Empty)";
        return new DoubleString(interactionMethod, name, false);
    }
    #endregion

    #region - Spell Reception -
    public virtual void TransferEffects(EffectHolder[] effects, string sourceName)
    {
        foreach (EffectHolder effect in effects)
        {
            if (effect.spellEffect is TrapObject)
            {
                acceptTraps = true;
            }
            else if (effect.spellEffect is Telekinesis)
            {
                Debug.Log("Not implemented yet");
                //Spring traps
            }
            else if (acceptTraps == true)
            {
                trapEffects.Add(effect);
            }

        }
        acceptTraps = false;
    }

    public virtual void OnDivinationEnter(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Traps && trapEffects.Count > 0)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red;
        }
    }

    public virtual void OnDivinationExit(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Traps)
        {
            outline.enabled = false;
        }
    }
    #endregion
}

//These should only be placed in player housing, but add a script or some function to save the items that are stored in the container