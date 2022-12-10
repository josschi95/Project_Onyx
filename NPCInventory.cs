using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInventory : CharacterInventory, IInteractable
{
    private CharacterStats stats;
    public float interactionDistance = 2f;
    [SerializeField] private bool hasBeenHarvested = false;

    protected override void Start()
    {
        base.Start();
        stats = GetComponent<CharacterStats>();
    }

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance)
        {
            if (stats.isDead || stats.isUnconscious) return true;
        }
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        if (stats.isDead || stats.isUnconscious) return true;
        return false;
    }

    public DoubleString GetInteractionDisplay()
    {
        string name = transform.name;

        if (!hasBeenHarvested && InputHandler.AlternateClick_Static())
        {
            if (stats.isUnconscious)
            {
                return new DoubleString("Kill and Harvest", name, false);
            }
            else if (stats.isDead)
            {
                return new DoubleString("Harvest", name, false);
            }
        }

        if (!isEmpty) CheckEmpty();
        if (isEmpty) name += " (Empty)";
        return new DoubleString("Search", name, false);
    }

    public void Interact(CharacterController controller)
    {
        if (InputHandler.AlternateClick_Static()) OnHarvest(controller);
        else OnSearch();
    }

    protected virtual void OnSearch()
    {
        CheckEmpty();
        if (isEmpty)
        {
            UIManager.instance.AddNotification("Found Nothing");
            //Play some kind of audio sound
            return;
        }
        //Play some kind of audio sound
        UIManager.instance.OpenLootContainer(this);
    }

    protected virtual void OnHarvest(CharacterController controller)
    {
        hasBeenHarvested = true;
        stats.ApplyDamage(controller.characterStats, stats.currentHealth, DamageType.NULL, true);

        //Add in harvestaable components

        if (isEmpty)
        {
            UIManager.instance.AddNotification("Found Nothing");
            //Play some kind of audio sound
            return;
        }
        //Play some kind of audio sound
        UIManager.instance.OpenLootContainer(this);
    }
}
