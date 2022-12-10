using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NPCDisplay : MonoBehaviour
{
    private Player_Settings settings;
    private HUDManager HUD;
    private CharacterStats stats;
    public GameObject canvas;
    public TMP_Text enemyName;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider healthBarBackground;
    [Space]
    [SerializeField] float smoothing = 50;
    [SerializeField] float delay = 0.5f;

    private Coroutine healthLag;
    private Coroutine rotateCoroutine;
    private bool tagModified = false;

    //0: Off 1: onHUD 2: overNPC
    private int hudSettings;

    private void Start()
    {
        settings = Player_Settings.instance;
        settings.onSettingsChanged += OnSettingsChanged;
        hudSettings = settings.npcHUD;

        HUD = HUDManager.instance;

        stats = GetComponentInParent<CharacterStats>();
        stats.onHealthChange += OnHitPointChange;

        enemyName.text = stats.name;
        healthBarBackground.maxValue = stats.statSheet.maxHealth.GetValue();
        healthBarBackground.value = healthBarBackground.maxValue;

        healthBar.maxValue = stats.statSheet.maxHealth.GetValue();
        healthBar.value = healthBar.maxValue;

        RectTransform rt = healthBar.GetComponent<RectTransform>();
        float barWidth = Mathf.Clamp(stats.statSheet.maxHealth.GetValue(), 100, 300);
        rt.sizeDelta = new Vector2(barWidth, 20);
        canvas.SetActive(false);
    }

    private void OnSettingsChanged()
    {
        if (settings.npcHUD == 0 || settings.npcHUD == 1)
        {
            canvas.SetActive(false);
            if (rotateCoroutine != null) StopCoroutine(rotateCoroutine);
            if (healthLag != null) StopCoroutine(healthLag);
        }
        hudSettings = settings.npcHUD;

        //stop all coroutines
        //set canvas inactive

    }

    private void OnHitPointChange(float amount)
    {
        //Don't display NPC HUD
        if (hudSettings == 0) return;
        //Display single NPC HUD on player HUD
        else if (hudSettings == 1)
        {
            //Pass the change along to the HUD
            HUD.enemyHUD.UpdateDisplay(stats);
        }
        //Display individual HUDs
        else
        {
            if (!canvas.activeSelf)
            {
                canvas.SetActive(true);
                if (rotateCoroutine != null) StopCoroutine(rotateCoroutine);
                rotateCoroutine = StartCoroutine(RotateHUD());
            }

            healthBar.value = stats.currentHealth;

            if (healthLag != null) StopCoroutine(healthLag);
            healthLag = StartCoroutine(HealthLag());
        }
    }

    private IEnumerator RotateHUD()
    {
        while (canvas.activeSelf)
        {
            canvas.transform.LookAt(PlayerManager.instance.player.transform);
            canvas.transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            yield return null;
        }
    }

    private IEnumerator HealthLag()
    {
        yield return new WaitForSeconds(delay);

        while (healthBarBackground.value != stats.currentHealth)
        {
            healthBarBackground.value = Mathf.MoveTowards(healthBarBackground.value, stats.currentHealth, smoothing * Time.deltaTime);
            yield return null;
        }

        if (stats.isDead)
        {
            canvas.SetActive(false);
        }
        else if (stats.isUnconscious && tagModified == false)
        {
            tagModified = true;
            enemyName.text = enemyName.text + " (Unconscious)";
        }
    }
}