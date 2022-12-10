using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossEventHandler : MonoBehaviour
{
    public delegate void OnNextEvent(int currentPhase);
    public OnNextEvent onNextEventCallback;

    [SerializeField] NPCStats stats;
    [SerializeField] private int currentPhase = 0;
    [SerializeField] int[] HPCheckpoints;

    Coroutine healthLag;
    [SerializeField] float smoothing = 15;
    [SerializeField] float delay = 1f;

    private BossBarUI bossBar;
    private TMP_Text bossName;
    private Slider healthBar;
    private Slider healthBarBackground;

    private void Start()
    {
        stats.onHealthChange += OnHitPointChange;

        bossBar = UIManager.instance.bossBar;
        healthBar = bossBar.healthBar;
        healthBarBackground = bossBar.healthBackground;
        bossName = bossBar.displayName;
    }

    public void OnBattleStart()
    {
        bossBar.gameObject.SetActive(true);
        bossName.text = transform.name;

        healthBarBackground.maxValue = stats.statSheet.maxHealth.GetValue();
        healthBarBackground.value = healthBarBackground.maxValue;

        healthBar.maxValue = stats.statSheet.maxHealth.GetValue();
        healthBar.value = healthBar.maxValue;
    }

    public void OnHitPointChange(float amount)
    {
        healthBar.value = stats.currentHealth;
        CheckForNextEvent();

        if (healthLag != null) StopCoroutine(healthLag);
        healthLag = StartCoroutine(HealthLag());
    }

    IEnumerator HealthLag()
    {
        yield return new WaitForSeconds(delay);

        while (healthBarBackground.value != stats.currentHealth)
        {
            healthBarBackground.value = Mathf.MoveTowards(healthBarBackground.value, stats.currentHealth, smoothing * Time.deltaTime);
            yield return null;
        }
        if (stats.currentHealth <= 0) bossBar.gameObject.SetActive(false);
    }

    private void CheckForNextEvent()
    {
        if (stats.currentHealth <= HPCheckpoints[currentPhase])
        {
            currentPhase++;
            if (onNextEventCallback != null)
            {
                onNextEventCallback.Invoke(currentPhase);
            }
        }
    }
}
