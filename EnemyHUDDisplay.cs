using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Class used to update the NPC Health Bar on the HUD
/// Only utilized if the player has the settings set to it
/// </summary>

public class EnemyHUDDisplay : MonoBehaviour
{
    private Player_Settings settings;
    public RectTransform display;
    public TMP_Text nameText;
    [SerializeField] private Slider healthBar;
    private CharacterStats currentCharacter;

    //0: Off 1: onHUD 2: overNPC
    private int hudSettings;
    //Automatically disable the HUD after delay
    private Coroutine disableCoroutine;

    private void Start()
    {
        settings = Player_Settings.instance;
        hudSettings = settings.npcHUD;
        settings.onSettingsChanged += OnSettingsChanged;
        display.gameObject.SetActive(false);
    }

    private void OnSettingsChanged()
    {
        //Player didn't change the NPC HUD settings
        if (settings.npcHUD == hudSettings) return;

        //If the HUD settings are set to off or individual, disable this
        if (settings.npcHUD == 0 || settings.npcHUD == 2)
        {
            DisableDisplay();
        }

        hudSettings = settings.npcHUD;
    }

    public void UpdateDisplay(CharacterStats character)
    {
        if (hudSettings == 0 || hudSettings == 2) return;

        if (character.isDead || character.isUnconscious)
        {
            DisableDisplay();
            return;
        }

        if (!display.gameObject.activeSelf) display.gameObject.SetActive(true);
        //Debug.Log("Enabling");

        if (currentCharacter != null)
        {
            if (character == currentCharacter)
            {
                //Debug.Log("same char");
                healthBar.maxValue = character.statSheet.maxHealth.GetValue();
                healthBar.value = character.currentHealth;
            }
            else OnNewDisplay(character);
        }
        else OnNewDisplay(character);

        if (disableCoroutine != null) StopCoroutine(disableCoroutine);
        disableCoroutine = StartCoroutine(TimeToDisable());
    }

    //Update the HUD Display for the most recent character
    private void OnNewDisplay(CharacterStats character)
    {
        if (currentCharacter != null) ClearCurrentCharacter();

        currentCharacter = character;
        character.onDeath += DisableDisplay;
        character.onUnconscious += DisableDisplay;

        nameText.text = character.name;
        float max = character.statSheet.maxHealth.GetValue();

        float barWidth = Mathf.Clamp(max * 2, 200, 1000);
        display.sizeDelta = new Vector2(barWidth, display.sizeDelta.y);

        healthBar.maxValue = max;
        healthBar.value = character.currentHealth;
    }

    private void DisableDisplay()
    {
        display.gameObject.SetActive(false);
        //Debug.Log("Disabling");
        ClearCurrentCharacter();

        if (disableCoroutine != null) StopCoroutine(disableCoroutine);
    }

    private void ClearCurrentCharacter()
    {
        if (currentCharacter == null) return;
        currentCharacter.onDeath -= DisableDisplay;
        currentCharacter.onUnconscious -= DisableDisplay;
        currentCharacter = null;
    }

    //There has not been any change in the last 60 seconds, disable the HUD
    private IEnumerator TimeToDisable()
    {
        yield return new WaitForSeconds(60);
        display.gameObject.SetActive(false);
        ClearCurrentCharacter();
    }

}
