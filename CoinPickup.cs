using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour, IInteractable
{
    public int goldQuantity;
    public int silverQuantity;
    public int copperQuantity;
    [Space]
    public string interactionMethod = "Take";
    public string pickupName = "Coins";
    public float interactionDistance = 2;

    public NPC owner;

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
        string method = interactionMethod;
        string target = pickupName;
        bool isCrime = false;

        if (owner != null)
        {
            method = "Steal";
            isCrime = true;
        }

        return new DoubleString(method, pickupName, isCrime);
    }

    public void Interact(CharacterController controller)
    {
        if (copperQuantity > 0) controller.inventory.AddCurrency(CurrencyType.Copper, copperQuantity);
        if (silverQuantity > 0) controller.inventory.AddCurrency(CurrencyType.Silver, silverQuantity);
        if (goldQuantity > 0) controller.inventory.AddCurrency(CurrencyType.Gold, goldQuantity);
        if (copperQuantity + silverQuantity + goldQuantity > 1) AudioManager.instance.Play("pickup_coins");
        else AudioManager.instance.Play("pickup_coin");
        Destroy(gameObject);
    }
}
