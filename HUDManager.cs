using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    #region - Singleton - 
    public static HUDManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of HUD Manager found");
            return;
        }
        instance = this;
    }
    #endregion

    #region - References -
    private PlayerEquipmentManager equip;
    private PlayerInventory inventory;
    private Player_Settings settings;
    private PlayerStats stats;
    public QuickslotManager quickslotManager;
    public ActiveEffectsDisplay activeEffectManager;
    public CompassBar compassBar;
    public EnemyHUDDisplay enemyHUD;
    [Space]
    #endregion

    public RectTransform statDisplay;
    public RectTransform compassDisplay;
    public RectTransform questDisplay;
    public RectTransform effectDisplay;
    public RectTransform ammoDisplay;
    [Space]
    public Slider healthBar;
    public Slider staminaBar;
    public Slider manaBar;
    public Slider healthBarBackground;
    public Slider tempHPBar;
    public Slider tempSPBar;
    public Slider tempMPBar;
    [Space]
    public TMP_Text hpText;
    public TMP_Text spText;
    public TMP_Text mpText;
    public TMP_Text ammoTextMainhand;
    public TMP_Text ammoTextOffhand;
    [Space]
    [SerializeField] private float smoothing = 15;
    [SerializeField] private float healthDelay = 3f;
    [SerializeField] private float timeToMove = 0.25f;

    #region - Aux Variables -
    [HideInInspector] public bool hudLocked;
    //Player Settings
    private bool displayHUD;
    private bool displayStatNumbers;

    private Coroutine HPDelayCoroutine;
    private Coroutine toggleCoroutine;
    private Vector2 statShownPos;
    private Vector2 statHiddenPos;
    private Vector2 ammoShownPos;
    private Vector2 ammoHiddenPos;

    private int[] smallStatBars = { 300, 100 };
    private int[] medStatBars = { 450, 150 };
    private int[] largeStatBars = { 600, 200 };
    #endregion

    private void Start()
    {
        equip = PlayerEquipmentManager.instance;
        inventory = PlayerInventory.instance;
        settings = Player_Settings.instance;
        stats = PlayerStats.instance;

        healthBarBackground.maxValue = stats.statSheet.maxHealth.GetValue();
        healthBarBackground.value = stats.currentHealth;
        healthBar.maxValue = stats.statSheet.maxHealth.GetValue();
        healthBar.value = stats.currentHealth;

        staminaBar.maxValue = stats.statSheet.maxHealth.GetValue();
        staminaBar.value = stats.currentStamina;

        manaBar.maxValue = stats.statSheet.maxHealth.GetValue();
        manaBar.value = stats.currentMana;

        ammoTextMainhand.text = "";
        ammoTextOffhand.text = "";

        //Hide or Show the HUD when the game is paused/resumed
        GameMaster.instance.onGamePaused += delegate { ToggleAllActiveElements(true); };
        GameMaster.instance.onGameResumed += delegate { ToggleAllActiveElements(false); };

        settings.onSettingsChanged += OnSettingsChange;

        equip.onWeaponSetChange += delegate { OnAmmunitionChange(); };
        inventory.onItemChangedCallback += delegate { OnAmmunitionChange(); };

        stats.onHealthChange += OnHitPointChange;
        stats.onManaChange += OnManaChange;
        stats.onStaminaChange += OnStaminaChange;

        statShownPos = statDisplay.anchoredPosition;
        statHiddenPos = statShownPos;
        statHiddenPos.y -= 200;

        ammoShownPos = ammoDisplay.anchoredPosition;
        ammoHiddenPos = ammoShownPos;
        ammoHiddenPos.y -= 200;

        OnSettingsChange();
    }

    private void OnSettingsChange()
    {
        displayStatNumbers = settings.displayStatNumbers;
        displayHUD = settings.displayHUD;

        ToggleAllActiveElements(!displayHUD);

        if (displayStatNumbers == true)
        {
            hpText.text = "HP " + Mathf.RoundToInt(stats.currentHealth) + " / " + stats.statSheet.maxHealth.GetValue().ToString();
            spText.text = "SP " + Mathf.RoundToInt(stats.currentStamina) + " / " + stats.statSheet.maxStamina.GetValue().ToString();
            mpText.text = "MP " + Mathf.RoundToInt(stats.currentMana) + " / " + stats.statSheet.maxMana.GetValue().ToString();
        }
        else
        {
            hpText.text = "";
            spText.text = "";
            mpText.text = "";
        }

    }

    public void ToggleAllActiveElements(bool hide)
    {
        if (!hide)
        {
            if (hudLocked || !displayHUD) return;
        }

        quickslotManager.ToggleQuickslots(hide);
        compassBar.ToggleCompassBar(hide);

        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);
        toggleCoroutine = StartCoroutine(ToggleStatBarCoroutine(hide));
    }

    public void HideImmediate()
    {
        quickslotManager.HideImmediate();
        compassBar.HideImmediate();
        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);

        statDisplay.anchoredPosition = statHiddenPos;
        ammoDisplay.anchoredPosition = ammoHiddenPos;
    }

    private void OnAmmunitionChange()
    {
        ammoTextMainhand.text = "";
        ammoTextOffhand.text = "";
        ammoTextMainhand.gameObject.SetActive(false);
        ammoTextOffhand.gameObject.SetActive(false);

        if (equip.projectilesMain != null)
        {
            var weapon = equip.projectilesMain;
            if (weapon.weaponType == WeaponType.Thrown)
            {
                ammoTextMainhand.text = weapon.name + " (" + inventory.QueryItemCount(weapon.itemID) + ")";
                ammoTextMainhand.gameObject.SetActive(true);
            }
        }

        if (equip.projectilesOff != null)
        {
            var weapon = equip.projectilesOff;
            if (weapon.weaponType == WeaponType.Thrown || weapon.weaponType == WeaponType.Arrow)
            {
                ammoTextOffhand.text = weapon.name + " (" + inventory.QueryItemCount(weapon.itemID) + ")";
                ammoTextOffhand.gameObject.SetActive(true);
            }
        }
    }

    #region - HP SP & MP -
    public void OnHitPointChange(float amount)
    {
        int maxValue = stats.statSheet.maxHealth.GetValue();

        healthBar.maxValue = maxValue;
        healthBar.value = stats.currentHealth;
        tempHPBar.maxValue = healthBar.maxValue;

        if (maxValue < stats.statSheet.maxHealth.Base_Value)
        {
            tempHPBar.value = stats.statSheet.maxHealth.Base_Value - maxValue;
        }
        else tempHPBar.value = 0;

        if (displayStatNumbers == true)
        {
            hpText.text = "HP " + Mathf.RoundToInt(stats.currentHealth) + " / " + maxValue;
        }
        else hpText.text = "";

        if (amount > 0) healthBarBackground.value = healthBar.value;
        else
        {
            if (HPDelayCoroutine != null) StopCoroutine(HPDelayCoroutine);
            HPDelayCoroutine = StartCoroutine(HealthLag());
        }
    }

    public void OnStaminaChange(float amount)
    {
        int maxValue = stats.statSheet.maxStamina.GetValue();

        staminaBar.maxValue = maxValue;
        tempSPBar.maxValue = staminaBar.maxValue;

        if (maxValue < stats.statSheet.maxStamina.Base_Value)
        {
            tempSPBar.value = stats.statSheet.maxStamina.Base_Value - maxValue;
        }
        else tempSPBar.value = 0;

        if (displayStatNumbers == true)
        {
            spText.text = "SP " + Mathf.RoundToInt(stats.currentStamina) + " / " + maxValue;
        }
        else spText.text = "";

        if (amount > 0) staminaBar.value = stats.currentStamina;
        else StartCoroutine(StaminaLag());
    }

    public void OnManaChange(float amount)
    {
        int maxValue = stats.statSheet.maxMana.GetValue();

        manaBar.maxValue = maxValue;
        tempMPBar.maxValue = manaBar.maxValue;

        if (maxValue < stats.statSheet.maxMana.Base_Value)
        {
            tempMPBar.value = stats.statSheet.maxMana.Base_Value - maxValue;
        }
        else tempMPBar.value = 0;

        if (displayStatNumbers == true)
        {
            mpText.text = "MP " + Mathf.RoundToInt(stats.currentMana) + " / " + maxValue;
        }
        else mpText.text = "";

        if (amount > 0) manaBar.value = stats.currentMana;
        else StartCoroutine(ManaLag());
    }

    private IEnumerator HealthLag()
    {
        yield return new WaitForSeconds(healthDelay);

        while (healthBarBackground.value != stats.currentHealth)
        {
            healthBarBackground.value = Mathf.MoveTowards(healthBarBackground.value, stats.currentHealth, smoothing * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator StaminaLag()
    {
        while (staminaBar.value != stats.currentStamina)
        {
            staminaBar.value = Mathf.MoveTowards(staminaBar.value, stats.currentStamina, smoothing * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ManaLag()
    {
        float newSmooth = smoothing;
        if ((manaBar.value - stats.currentMana) > 6) newSmooth = smoothing * 3f;
        else newSmooth = smoothing * 0.1f;

        while (manaBar.value != stats.currentMana)
        {
            manaBar.value = Mathf.MoveTowards(manaBar.value, stats.currentMana, newSmooth * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    private IEnumerator ToggleStatBarCoroutine(bool hide)
    {
        //Debug.Log("HUD Toggled " + hide);
        float elapsedTime = 0;
        var statStart = statDisplay.anchoredPosition;
        var ammoStart = ammoDisplay.anchoredPosition;
        var statEnd = statShownPos;
        var ammoEnd = ammoShownPos;

        if (hide == true)
        {
            statEnd = statHiddenPos;
            ammoEnd = ammoHiddenPos;
        }

        while (elapsedTime < timeToMove)
        {
            statDisplay.anchoredPosition = Vector2.Lerp(statStart, statEnd, (elapsedTime / timeToMove));
            ammoDisplay.anchoredPosition = Vector2.Lerp(ammoStart, ammoEnd, (elapsedTime / timeToMove));
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        statDisplay.anchoredPosition = statEnd;
        ammoDisplay.anchoredPosition = ammoEnd;
    }

    public void OnNPCDamaged(CharacterStats character)
    {

    }
}