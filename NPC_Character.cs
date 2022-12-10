using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Character : MonoBehaviour, IInteractable
{
    private NPCController controller;
    private CharacterStats stats;
    public NPC character;
    private NPCInventory inventory;
    [Space]
    public float interactionDistance = 2;
    public string interactionDisplay = "Talk";

    private void Start()
    {
        stats = GetComponent<CharacterStats>();
        controller = GetComponent<NPCController>();
        stats.onDeath += OnDeath;
        inventory = GetComponent<NPCInventory>();
    }

    private void OnDeath()
    {
        character.OnCharacterDeath();
    }
    
    private bool OnParleyAttempt(PlayerStats playerStats)
    {
        float chance = 50 + playerStats.statSheet.speech.GetStatDecim();

        if (controller.isHostile) chance -= 25; //Is this even necessary? if it's a parley they WILL be hostile

        //swap this out for hostility when I implement that
        //decrease chance with hostility
        //increase chance with fear

        if (playerStats.characterCombat.weaponsDrawn) chance -= 25;

        if (character.primaryLanguage != SpokenLanguage.Common)
        {
            if (playerStats.PlayerSpeaksLanguage(character.primaryLanguage) == false) chance -= 25;
            else chance += 25 + playerStats.statSheet.linguistics.GetStatDecim();
        }

        int hp = stats.statSheet.maxHealth.GetValue();
        //target has taken damage
        if (stats.currentHealth < hp)
        {
            chance -= 25;
            if (stats.currentHealth <= (hp / 2)) chance -= 25;
        }

        //I don't actually know what I'm going to compare this against yet, maybe hostility?
        //This is just a holder for now
        if (chance >= 50) return true;
        UIManager.instance.AddNotification("Parley Failed");
        return false;
    }

    #region - IInteractable -
    public bool DisplayPopup(float distance)
    {
        if (stats.isDead || stats.isUnconscious)
        {
            if (distance <= interactionDistance && inventory != null) return true;
            return false;
        }
        else if (controller.isHostile && !InputHandler.AlternateClick_Static()) return false;
        else if (InputHandler.AlternateClick_Static() && distance <= 15 && PlayerCombat.instance.weaponsDrawn == false) return true;
        else if (distance > interactionDistance) return false;
        else return true;
    }

    public DoubleString GetInteractionDisplay()
    {
        if (stats.isDead || stats.isUnconscious)
        {
            return inventory.GetInteractionDisplay();
        }

        string method = interactionDisplay;
        if (controller.isHostile) method = "Parley";
        return new DoubleString(character.name, method, false);
    }

    public bool CanBeAccessed(float distance)
    {
        if (stats.isDead || stats.isUnconscious)
        {
            if (inventory != null) return true;
            return false;
        }
        if (controller.isInteracting) return false;
        return true;
    }

    public void Interact(CharacterController controller)
    {
        if (stats.isDead || stats.isUnconscious)
        {
            inventory.Interact(controller);
        }
        else
        {
            UIManager.instance.OnDialogueStart(character, character.name, gameObject.transform);
            this.controller.conversant = controller;
        }
    }
    #endregion
}