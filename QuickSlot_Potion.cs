using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuickSlot_Potion : QuickSlot
{
    PlayerStats playerStats;
    PlayerInventory inventory;
    [SerializeField] Image cooldownDisplay;
    public TMP_Text quantityDisplay;
    public Potion potion;

    Coroutine cooldownTimer;

    private void Start()
    {
        playerStats = PlayerStats.instance;
        inventory = PlayerInventory.instance;
        playerStats.onPotionUsed += OnCooldown;

        OnAssignPotion(null);
    }

    public void OnAssignPotion(Potion newPotion)
    {
        if (newPotion == null)
        {
            potion = null;
            slotImage.sprite = null;
            cooldownDisplay.sprite = null;
            slotImage.enabled = false;
            cooldownDisplay.enabled = false;
            slotName.text = "";
            quantityDisplay.text = "";
        }
        else
        {
            potion = newPotion;
            slotImage.enabled = true;
            cooldownDisplay.enabled = true;
            slotImage.sprite = newPotion.icon;
            cooldownDisplay.sprite = newPotion.icon;
            slotName.text = newPotion.name;
            quantityDisplay.text = PlayerInventory.instance.QueryItemCount(newPotion.itemID).ToString();
        }
    }

    public override void OnUse()
    {
        if (potion == null)
        {
            UIManager.instance.DisplayPopup("No Assigned Potion");
            return;
        }
        playerStats.OnUsePotion(potion);
        quantityDisplay.text = inventory.QueryItemCount(potion.itemID).ToString();
    }

    public void OnCooldown()
    {
        if (cooldownTimer != null) StopCoroutine(cooldownTimer);
        cooldownTimer = StartCoroutine(CooldownTimer(playerStats.potionCooldown));
    }

    private IEnumerator CooldownTimer(float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            cooldownDisplay.fillAmount = 1 - (elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
