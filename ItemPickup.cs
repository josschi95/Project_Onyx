using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public Item item;
    public int quantity = 1;
    [Space]
    [Tooltip("If not null, taking this item is a crime")]
    public NPC owner;

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
        controller.inventory.AddItem(item, quantity);
        PlaySound();
        Destroy(gameObject); //Pool?
    }

    private void PlaySound()
    {

        switch (item.itemCategory)
        {
            case ItemCategory.APPAREL:
                {
                    AudioManager.instance.Play("pickup_apparel");
                    break;
                }
            case ItemCategory.WEAPON:
                {
                    AudioManager.instance.Play("pickup_weapon");
                    break;
                }
            case ItemCategory.POTION:
                {
                    AudioManager.instance.Play("pickup_potion");
                    break;
                }
            /*case ItemCategory.CRAFTING:
                {
                    AudioManager.instance.Play("pickup_crafting");
                    break;
                }*/
            case ItemCategory.MISC:
                {
                    var misc = item as Miscellaneous;
                    switch (misc.miscClass)
                    {
                        case MiscClass.Other:
                            {
                                AudioManager.instance.Play("pickup_misc");
                                break;
                            }
                        case MiscClass.Key:
                            {
                                AudioManager.instance.Play("pickup_key");
                                break;
                            }
                        case MiscClass.Gem:
                            {
                                AudioManager.instance.Play("pickup_gem");
                                break;
                            }
                        case MiscClass.Lockpick:
                            {
                                AudioManager.instance.Play("pickup_misc");
                                break;
                            }
                        case MiscClass.Treasure:
                            {
                                AudioManager.instance.Play("pickup_treasure");
                                break;
                            }
                    }
                    break;
                }
        }
    }

    public string interactionMethod = "Take";
    public DoubleString GetInteractionDisplay()
    {
        string method = interactionMethod;
        string target = item.name;
        bool isCrime = false;
        if (quantity > 1) target += " (" + quantity + ")"; 
        if (owner != null)
        {
            method = "Steal";
            isCrime = true;
        }
        return new DoubleString(method, target, isCrime);
    }
}
