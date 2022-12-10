using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot_Ability : QuickSlot
{
    PlayerStats playerStats;
    PlayerAnimator playerAnimator;
    [SerializeField] Image cooldownDisplay;
    public Ability ability;

    Coroutine cooldownTimer;

    private void Start()
    {
        playerStats = PlayerStats.instance;
        playerAnimator = playerStats.GetComponent<PlayerAnimator>();
        playerStats.onAbilityUsed += OnCooldown;

        OnAssignAbility(null);
    }

    public void OnAssignAbility(Ability newAbility)
    {
        if (playerStats == null) playerStats = PlayerStats.instance;

        if (newAbility == null)
        {
            ability = null;
            slotImage.sprite = null;
            cooldownDisplay.sprite = null;
            slotImage.enabled = false;
            cooldownDisplay.enabled = false;
            slotName.text = "";
            playerStats.currentAbility = null;
        }
        else
        {
            ability = newAbility;
            slotImage.enabled = true;
            cooldownDisplay.enabled = true;
            slotImage.sprite = newAbility.icon;
            cooldownDisplay.sprite = newAbility.icon;
            slotName.text = newAbility.name;
            playerAnimator.OnAbilityChange(newAbility);
            playerStats.currentAbility = newAbility;
        }
    }

    public override void OnUse()
    {
        if (ability == null)
        {
            UIManager.instance.DisplayPopup("No Assigned Ability");
            return;
        }
        playerStats.OnUseAbility(ability);
    }

    public void OnCooldown(Ability ability)
    {
        if (cooldownTimer != null) StopCoroutine(cooldownTimer);
        cooldownTimer = StartCoroutine(CooldownTimer(ability.cooldownDuration));
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
