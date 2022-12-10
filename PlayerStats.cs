using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class PlayerStats : CharacterStats, IDamageable
{
    #region - Singleton -
    public static PlayerStats instance;

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerStats found");
            return;
        }
        instance = this;
    }
    #endregion

    private PlayerPerkManager playerPerks;

    public bool resetStatsOnStart;

    //Can I just have a reference to these in here?
    public bool lastStandUsed = true;
    private bool secondWindUsed = false;

    public float manaCostMultiplier { get; private set; }
    private Coroutine manaModCoroutine;

    #region - Languages -
    [Header("Languages")]
    [Tooltip("Common, Demonic, Dwarvish, Elvish, Fey, Giant, Primordial, Hoarish, Beast")]
    public int[] spokenLanguages;
    #endregion

    protected override void Start()
    {
        base.Start();
        manaCostMultiplier = 1;
        playerPerks = GetComponent<PlayerPerkManager>();
        DateTimeManager.instance.onNewDay += delegate { lastStandUsed = false; };
        DateTimeManager.instance.onNewDay += delegate { secondWindUsed = false; };

        statSheet.barter.onStatValueChanged += OnStatValueModified;
        statSheet.enchantment.onStatValueChanged += OnStatValueModified;
        statSheet.intuition.onStatValueChanged += OnStatValueModified;

        statSheet.leadership.onStatValueChanged += OnStatValueModified;
        statSheet.linguistics.onStatValueChanged += OnStatValueModified;
        statSheet.speech.onStatValueChanged += OnStatValueModified;
        statSheet.CompileAllStats();
        if (resetStatsOnStart) statSheet.ResetScores();

        spokenLanguages = new int[System.Enum.GetNames(typeof(SpokenLanguage)).Length];
        int i = (int)SpokenLanguage.Common;
        spokenLanguages[i] = 1;
    }

    public bool SkillTest(Stat skill, int difficulty)
    {
        int playerSkill = skill.GetValue();
        if (playerSkill >= difficulty) return true;
        return false;
    }

    public void AddSkillExperience(Skill skill, int xp)
    {
        skill.GrantStatXP(xp);
    }

    #region - HP & MP -
    public override void RestoreHealth(float healAmount)
    {
        
        if (playerPerks.HasPerk(playerPerks.perkManager.fastHealer_v2))
        {
            healAmount *= 1.2f;
        }
        else if (playerPerks.HasPerk(playerPerks.perkManager.fastHealer_v1))
        {
            healAmount *= 1.1f;
        }
        
        base.RestoreHealth(healAmount);
    }

    public void OnManaModifierChange(float newMod, float duration)
    {
        if (manaModCoroutine != null) StopCoroutine(manaModCoroutine);
        manaModCoroutine = StartCoroutine(DecreaseManaCost(newMod, duration));
    }

    IEnumerator DecreaseManaCost(float newMod, float duration)
    {
        manaCostMultiplier = newMod;
        yield return new WaitForSeconds(duration);
        manaCostMultiplier = 1;
    }

    public override bool CheckManaCost(float manaCost)
    {
        if (manaCost <= currentMana) return true;
        else
        {
            if (playerPerks.HasPerk(playerPerks.perkManager.bloodMagic) && manaCost < currentMana + currentHealth)
            {
                return true;
            }
            else if (playerPerks.HasPerk(playerPerks.perkManager.drainTheWell) && manaCost < currentMana + statSheet.maxMana.GetValue())
            {
                return true;
            }
            else return false;
        }
    }

    public override void SpendMana(float manaAmount)
    {
        manaAmount *= manaCostMultiplier;
        if (manaAmount > currentMana)
        {
            float num = manaAmount - currentMana;
            if (playerPerks.HasPerk(playerPerks.perkManager.bloodMagic))
            {
                currentHealth -= num;
                manaAmount -= num;
            }
            else if (playerPerks.HasPerk(playerPerks.perkManager.drainTheWell))
            {
                Debug.Log("This won't work, possibly change to ability to channel into Mana");
                //maxMana.AddNamedModifier("Drain The Well", -num);
                //manaAmount -= num;
            }
        }
        float netMana = -manaAmount;
        if (manaAmount > currentMana) netMana = -currentMana;

        currentMana -= manaAmount;
        if (currentMana < 0) currentMana = 0;
        if (onManaChange != null) onManaChange.Invoke(netMana);

        float t = regenDelay;
        if (currentMana == 0) t *= 1.5f;

        if (manaRegenCoroutine != null) StopCoroutine(manaRegenCoroutine);
        manaRegenCoroutine = StartCoroutine(ManaRegeneration(t));
    }
    #endregion

    #region - Abilities -
    public bool canUseAbility = true;
    public float abilityCooldown;
    Coroutine abilityCooldownCoroutine;

    public void OnUseAbility(Ability ability)
    {
        if (canUseAbility == false)
        {
            UIManager.instance.DisplayPopup("Cannot Use Ability");
            return;
        }

        animator.Play("ability");

        canUseAbility = false;
        float cooldown = ability.cooldownDuration;

        if (abilityCooldownCoroutine != null) StopCoroutine(abilityCooldownCoroutine);
        abilityCooldownCoroutine = StartCoroutine(AbilityCooldown(cooldown));

        if (onAbilityUsed != null) onAbilityUsed.Invoke(ability);
    }

    IEnumerator AbilityCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        canUseAbility = true;
    }
    #endregion

    public override void Die()
    {
        if (isDead == true) return;

        if (PlayerPerkManager.instance.HasPerk(PlayerPerkManager.instance.perkManager.lastStand) && lastStandUsed == false)
        {
            currentHealth = statSheet.maxHealth.GetValue() * 0.1f;
            //Play some kind of ParticleFX here
            lastStandUsed = true;
            if (onHealthChange != null) onHealthChange.Invoke(currentHealth);
            return;
        }
        else if (PlayerPerkManager.instance.HasPerk(PlayerPerkManager.instance.perkManager.secondWind) && secondWindUsed == false)
        {
            currentHealth = statSheet.maxHealth.GetValue();
            //Play some kind of ParticleFX here
            secondWindUsed = true;
            if (onHealthChange != null) onHealthChange.Invoke(currentHealth);
            return;
        }

        isDead = true;
        animator.SetTrigger("die");
        PlayerManager.instance.KillPlayer();
    }

    protected override void OnStatValueModified(Stat stat, int magnitude)
    {
        base.OnStatValueModified(stat, magnitude);
        if (stat == statSheet.barter) DialogueLua.SetVariable("barter", PlayerStats.instance.statSheet.barter.GetValue());
        else if (stat == statSheet.enchantment) DialogueLua.SetVariable("enchantment", PlayerStats.instance.statSheet.enchantment.GetValue());
        else if (stat == statSheet.intuition) DialogueLua.SetVariable("intuition", PlayerStats.instance.statSheet.intuition.GetValue());
        else if (stat == statSheet.leadership) DialogueLua.SetVariable("leadership", PlayerStats.instance.statSheet.leadership.GetValue());
        else if (stat == statSheet.linguistics) DialogueLua.SetVariable("linguistics", PlayerStats.instance.statSheet.linguistics.GetValue());
        else if (stat == statSheet.speech) DialogueLua.SetVariable("speech", PlayerStats.instance.statSheet.speech.GetValue());
    }

    public void LearnLanguage(SpokenLanguage language)
    {
        int i = (int)language;
        spokenLanguages[i] = 1;
    }

    public bool PlayerSpeaksLanguage(SpokenLanguage language)
    {
        int i = (int)language;
        if (spokenLanguages[i] == 0) return false;
        return true;
    }

    public void SetSavedValues(float hp, float mp, float sp)
    {
        currentHealth = hp;
        currentMana = mp;
        currentStamina = sp;
        if (onHealthChange != null) onHealthChange.Invoke(0);
        if (onManaChange != null) onManaChange.Invoke(0);
        if (onStaminaChange != null) onStaminaChange.Invoke(0);
        Debug.Log("Saved values set");
    }
}
