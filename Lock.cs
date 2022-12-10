using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lock : MonoBehaviour
{
    [SerializeField] Key key;
    public int difficulty;
    public bool isLocked = false;
    public bool keyRequired = false; //Lock can only be opened with a key

    public bool LockCascade()
    {
        if (PlayerHasKey())
        {
            //Play unlock sound
            isLocked = false;
            return true;
        }
        else if (key != null)
        {
            UIManager.instance.DisplayPopup("RQUIRES KEY");
            return false;
        }
        else
        {
            if (PlayerInventory.instance.lockpicks > 0)
            {
                var player = PlayerStats.instance;
                if (player.SkillTest(player.statSheet.thievery, difficulty))
                {
                    UIManager.instance.DisplayPopup("Lockpicking Successful!");
                    isLocked = false;
                    return true;
                }
                else
                {
                    UIManager.instance.DisplayPopup("Lockpicking Failed.");
                    PlayerInventory.instance.RemoveItem(PlayerInventory.instance.lockpick, 1);
                    return false;
                }
            }
            else
            {
                UIManager.instance.DisplayPopup("CANNOT OPEN " + Environment.NewLine + gameObject.name); // \n in string
                return false;
            }
        }
    }

    public bool PlayerHasKey()
    {
        if (key == null) return false;

        foreach (InventoryItem inventoryItem in PlayerInventory.instance.miscellaneous)
        {
            if (inventoryItem != null && inventoryItem.itemID == key.itemID) return true;

            //Transitioning to itemID based system
            /*if (inventoryItem != null && inventoryItem.item is Key playerKey)
            {
                if (playerKey == key) return true;
            }*/
        }
        return false;
    }
}