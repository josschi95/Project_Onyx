using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour, IDamageable
{
    #region - References -
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public EquipmentManager equipmentManager;
    [HideInInspector] public CharacterCombat characterCombat;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;
    protected AudioSource audioSource;
    #endregion
    
    public CharacterStatSheet statSheet;
    public List<ActiveEffect> activeEffects = new List<ActiveEffect>();

    [HideInInspector] public Ability currentAbility;
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isUnconscious = false;
    [Tooltip("If false, dies when HP is reduced to 0")]
    public bool canBeKnockedUnconscious = true;

    public float currentHealth { get; protected set; }
    public float currentStamina { get; protected set; }
    public float currentMana { get; protected set; }

    #region - Coroutine -
    protected float regenDelay = 5f;
    protected Coroutine healthRegenCoroutine;
    protected Coroutine staminaRegenCoroutine;
    protected Coroutine manaRegenCoroutine;
    protected Coroutine invincibilityCoroutine;
    #endregion

    #region - Callbacks -
    public delegate void OnHealthChange(float amount);
    public OnHealthChange onHealthChange;

    public delegate void OnDamageTaken(CharacterStats attacker, float damage, DamageType type, bool isLethal);
    public OnDamageTaken onDamageTaken;

    public delegate void OnStaminaChange(float amount);
    public OnStaminaChange onStaminaChange;

    public delegate void OnManaChange(float amount);
    public OnManaChange onManaChange;

    public delegate void OnDeath();
    public OnDeath onDeath;

    public delegate void OnUnconscious();
    public OnUnconscious onUnconscious;

    public delegate void OnPotionUsed();
    public OnPotionUsed onPotionUsed;

    public delegate void OnAbilityUsed(Ability ability);
    public OnAbilityUsed onAbilityUsed;
    #endregion

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        equipmentManager = GetComponent<EquipmentManager>();
        characterCombat = GetComponent<CharacterCombat>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        currentHealth = statSheet.maxHealth.Base_Value;
        currentMana = statSheet.maxMana.Base_Value;
        currentStamina = statSheet.maxStamina.Base_Value;

        equipmentManager.onEquipmentChanged += OnEquipmentChanged;

        statSheet.maxHealth.onStatValueChanged += OnStatValueModified;
        statSheet.maxStamina.onStatValueChanged += OnStatValueModified;
        statSheet.maxMana.onStatValueChanged += OnStatValueModified;

        statSheet.healthRegenRate.onStatValueChanged += OnStatValueModified;
        statSheet.staminaRegenRate.onStatValueChanged += OnStatValueModified;
        statSheet.manaRegenRate.onStatValueChanged += OnStatValueModified;

        statSheet.strength.onStatValueChanged += OnStatValueModified;
        statSheet.armor.onStatValueChanged += OnStatValueModified;
    }

    protected virtual void RecalculateArmorRating()
    {
        int armor = 0;
        //0-3 are always weapons
        for (int i = 4; i < equipmentManager.currentEquipment.Length; i++)
        {
            //if (equipmentManager.currentEquipment[i] != null && equipmentManager.currentEquipment[i].item is Apparel apparel)
            if (equipmentManager.currentEquipment[i] != null && DatabaseManager.GetItem(equipmentManager.currentEquipment[i].itemID) is Apparel apparel)
            {
                int value = apparel.armor;
                switch (apparel.armorSkill)
                {
                    case ArmorSkill.Unarmored:
                        {
                            value *= statSheet.unarmored.GetStatDecim();
                            break;
                        }
                    case ArmorSkill.Light:
                        {
                            value *= statSheet.lightArmor.GetStatDecim();
                            break;
                        }
                    case ArmorSkill.Medium:
                        {
                            value *= statSheet.mediumArmor.GetStatDecim();
                            break;
                        }
                    case ArmorSkill.Heavy:
                        {
                            value *= statSheet.heavyArmor.GetStatDecim();
                            break;
                        }
                }
                armor += value;                
            }
        }
        statSheet.armor.SetBaseValue(armor);
    }

    #region - IDamageable -
    public virtual void ApplyDamage(CharacterStats attacker, float amount, DamageType type, bool isLethal)
    {
        if (isInvincible == true || isDead == true) return;
        if (DamageEvaded()) return;

        float totalDamage = (DamageReduction(amount, type));

        BloodParticles(type);
        StartCoroutine(HitFreeze());
        SpendHealth(totalDamage);

        //Debug.Log(transform.name + " takes " + totalDamage + " " + type.ToString() + " damage");

        if (currentHealth < 1)
        {
            currentHealth = 0;
            if (canBeKnockedUnconscious == true && isLethal == false)
            {
                Unconscious();
            }
            else Die();
        }
        if (onHealthChange != null) onHealthChange.Invoke(-totalDamage);
        if (onDamageTaken != null) onDamageTaken.Invoke(attacker, totalDamage, type, isLethal);
    }

    public virtual void ApplyDamageOverTime(float amount, DamageType type, bool isLethal, float duration)
    {
        if (duration == 0) ApplyDamage(null, amount, type, isLethal);
        else StartCoroutine(DamageOverTime(amount, type, isLethal, duration));
    }

    protected virtual IEnumerator DamageOverTime(float amount, DamageType type, bool isLethal, float duration)
    {
        float elapsedTime = 0;
        while(elapsedTime < duration)
        {
            ApplyDamage(null, amount * Time.deltaTime, type, isLethal);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region - Damage Reduction -
    public virtual bool DamageEvaded()
    {
        //So this is 1/4th of the total value, meaning at 100 EVA, there is a 1/4 chance of an attack missing
        //float smallerValue = statSheet.evasion.GetStatDecim(); //This would result in a "max" 10% chance of all attacks missing
        float value = statSheet.evasion.GetValue() * 0.25f; //people like math so I could throw in Finesse here as well, maybe apparel weight to make things needlessly complicated
        if (value > 50) value = 50;
        float chance = Random.Range(0, 100);
        if (chance <= value)
        {
            Debug.Log("Damage Evaded. evasion: " + value + ", chance rolled: " + chance);
            return true;
        }
        return false;
    }

    public virtual float DamageReduction(float amount, DamageType type)
    {
        float totalDamage = amount;
        if (characterCombat.isBlocking == true) totalDamage = OnBlock(amount, type);

        //Returns armor as a percent clamped to reduce damage up to 80%
        if ((int)type <= 5) totalDamage = ArmorDefense(totalDamage);

        totalDamage *= (1 - statSheet.GetResistance((TypeResistance)(int)type).GetValue() * 0.01f);
        //totalDamage *= (1 - statSheet.GetDamageResistance(type).GetValue() * 0.01f);
        return totalDamage;
    }

    protected virtual float OnBlock(float damage, DamageType damageType)
    {
        float blockPercent = 0;
        float spentStamina = Mathf.Clamp(damage - statSheet.shields.GetStatDecim(), 0, damage);

        SpendStamina(spentStamina);
        if (damage > statSheet.shields.GetStatDecim() && currentStamina < 1) animator.Play("crit");
        else if (currentStamina < 1) animator.Play("stagger");
        else if (damage > statSheet.shields.GetStatDecim()) animator.SetTrigger("shieldStruck");

        //If the shield is magic, it will block any damage type
        if (characterCombat.shield != null && characterCombat.shield.isMagic) blockPercent = statSheet.shields.GetValue();
        //Else the shield (and possibly weapon in future) will only reduce physical damage
        else if ((int)damageType <= 2) blockPercent = statSheet.shields.GetValue();

        blockPercent = Mathf.Clamp(blockPercent, 0, 80);
        return damage * (1 - (blockPercent * 0.01f));
    }

    protected virtual float ArmorDefense(float amount)
    {
        float armorRating = Mathf.Clamp(1 - (statSheet.armor.GetValue() * 0.01f), 0.2f, 1f);
        return amount * armorRating;
    }
    #endregion

    #region - Health -
    protected virtual void SpendHealth(float magnitude)
    {
        currentHealth -= magnitude;

        if (healthRegenCoroutine != null) StopCoroutine(healthRegenCoroutine);
        healthRegenCoroutine = StartCoroutine(HealthRegeneration());
    }

    public virtual void RestoreHealth(float healAmount)
    {
        int maxValue = statSheet.maxHealth.GetValue();
        float netHeal = healAmount;
        if (currentHealth + healAmount > maxValue) 
            netHeal = maxValue - currentHealth;

        currentHealth += healAmount;
        if (currentHealth > maxValue) currentHealth = maxValue;
        if (onHealthChange != null) onHealthChange.Invoke(netHeal);
    }

    public virtual void FullHeal()
    {
        int maxValue = statSheet.maxHealth.GetValue();
        float netHeal = maxValue - currentHealth;
        currentHealth = maxValue;
        if (onHealthChange != null) onHealthChange.Invoke(netHeal);
    }

    protected IEnumerator HealthRegeneration()
    {
        float magnitude = statSheet.healthRegenRate.GetValue();
        if (magnitude <= 0) yield break;

        while (currentHealth < statSheet.maxHealth.GetValue())
        {
            RestoreHealth(magnitude * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region - Mana -
    public virtual bool CheckManaCost(float manaCost)
    {
        if (manaCost > currentMana) return false;
        else return true;
    }

    public virtual void SpendMana(float manaAmount)
    {
        float netMana = -manaAmount;
        if (manaAmount > currentStamina) netMana = -currentStamina;

        currentMana -= manaAmount;
        if (currentMana < 0) currentMana = 0;
        if (onManaChange != null) onManaChange.Invoke(netMana);

        float t = regenDelay;
        if (currentMana == 0) t *= 1.5f;

        if (manaRegenCoroutine != null) StopCoroutine(manaRegenCoroutine);
        manaRegenCoroutine = StartCoroutine(ManaRegeneration(t));
    }

    public virtual void RestoreMana(float manaAmount)
    {
        int maxValue = statSheet.maxMana.GetValue();
        float netMana = manaAmount;
        if (currentMana + manaAmount > maxValue)
            netMana = maxValue - currentMana;
        //So I'm doing this check twice to get the net amount, I think I can remove the lines below this here for HP/SP/MP
        currentMana += manaAmount;
        if (currentMana > maxValue) currentMana = maxValue;
        if (onManaChange != null) onManaChange.Invoke(netMana);
    }

    public virtual void FullMana()
    {
        int maxValue = statSheet.maxMana.GetValue();
        float netMana = maxValue - currentMana;
        currentMana = maxValue;
        if (onManaChange != null) onManaChange.Invoke(netMana);
    }

    protected IEnumerator ManaRegeneration(float delay)
    {
        yield return new WaitForSeconds(delay);
        float magnitude = statSheet.manaRegenRate.GetValue();
        if (magnitude <= 0) yield break;

        while (currentMana < statSheet.maxMana.GetValue())
        {
            RestoreMana(magnitude * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region - Stamina -
    public virtual bool CheckStaminaCost(float staCost)
    {
        if (staCost > currentStamina) return false;
        else return true;
    }

    public virtual void SpendStamina(float staAmount)
    {
        float netSta = -staAmount;
        if (staAmount > currentStamina) netSta = -currentStamina;

        currentStamina -= staAmount;
        if (currentStamina < 0) currentStamina = 0;
        if (onStaminaChange != null) onStaminaChange.Invoke(netSta);

        float t = regenDelay;
        if (currentMana == 0) t *= 1.5f;
        if (staminaRegenCoroutine != null) StopCoroutine(staminaRegenCoroutine);
        staminaRegenCoroutine = StartCoroutine(StaminaRegeneration(t));
    }

    public virtual void RestoreStamina(float staAmount)
    {
        int maxValue = statSheet.maxStamina.GetValue();
        float netSta = staAmount;
        if (currentStamina + staAmount > maxValue)
            netSta = maxValue - currentStamina;

        currentStamina += staAmount;
        if (currentStamina > maxValue) currentStamina = maxValue;
        if (onStaminaChange != null) onStaminaChange.Invoke(netSta);
    }

    public virtual void FullStamina()
    {
        int maxValue = statSheet.maxStamina.GetValue();
        float netSTa = maxValue - currentStamina;

        currentStamina = maxValue;
        if (onStaminaChange != null) onStaminaChange.Invoke(netSTa);
    }

    protected IEnumerator StaminaRegeneration(float delay)
    {
        yield return new WaitForSeconds(delay);
        float magnitude = statSheet.staminaRegenRate.GetValue();
        if (magnitude <= 0) yield break;

        while (currentStamina < statSheet.maxStamina.GetValue())
        {
            RestoreStamina(magnitude * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    public virtual void FullRestore()
    {
        FullHeal();
        FullMana();
        FullStamina();
    }

    public virtual void SetInvincible(float time)
    {
        if (invincibilityCoroutine != null) StopCoroutine(invincibilityCoroutine);
        invincibilityCoroutine = StartCoroutine(Invincible(time));
    }

    public virtual IEnumerator Invincible(float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            isInvincible = true;
            Debug.Log("isInvincible");
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }        
        isInvincible = false;
        Debug.Log("is not invincible");
    }

    #region - Damage Response - 
    protected virtual void BloodParticles(DamageType damageType)
    {
        //Might later change this where each creature can get their own string for spawned effect
        //e.g. a slime would get same but recolored green, skeletons might get same but recolored gray, etc.
        if ((int)damageType <= 2) //Damage type was physical, and it was not 0
            ObjectPooler.SpawnFromPool_Static("bloodSmall", characterController.center.position, Quaternion.identity);
    }

    //Add knockback force to target, disable navmeshagent if NPC
    public virtual void Knockback(Vector3 attackOrigin, float knockbackForce)
    {
        Vector3 pushDirection = attackOrigin - rb.transform.position;
        rb.AddForce(pushDirection.normalized * -knockbackForce);
    }

    public IEnumerator HitFreeze()
    {
        animator.speed = 0.05f;
        yield return new WaitForSeconds(0.1f);
        animator.speed = 1;
    }

    public virtual void CriticalHitTaken()
    {
        animator.Play("crit");
    }

    public virtual void Die()
    {
        Debug.Log(transform.name + " died");
        if (isDead == true) return;

        isDead = true;
        if (onDeath != null) onDeath.Invoke();
        Debug.Log("invoking onDeath");
    }

    public virtual void Unconscious()
    {
        if (isUnconscious == true) return;

        isUnconscious = true;
        if (onUnconscious != null)
            onUnconscious.Invoke();
    }
    #endregion

    #region - Potions -
    [HideInInspector] public bool canUsePotion = true;
    [HideInInspector] public float potionCooldown = 5;
    protected float basePotionCooldown = 5f;
    protected Coroutine totalPotionCooldown;

    public bool OnUsePotion(Potion potion)
    {
        if (canUsePotion == false)
        {
            if (this is PlayerStats)
            {
                UIManager.instance.DisplayPopup("Cannot Use Potion");
            }
            
            return false;
        }

        if (potion is Poison poison)
        {
            if (this is PlayerStats)
            {
                poison.PlayerPoisonOptions(characterCombat);
            }
            else
            {
                if (equipmentManager.mainHand != null && poison.WeaponCanBePoisoned(equipmentManager.mainHand))
                {
                    poison.ApplyWeaponPoison(equipmentManager.mainHand);
                }
                else if (equipmentManager.offHand != null && poison.WeaponCanBePoisoned(equipmentManager.offHand))
                {
                    poison.ApplyWeaponPoison(equipmentManager.offHand);
                }
            }
        }
        else
        {
            for (int i = 0; i < potion.potionEffects.Count; i++)
            {
                potion.potionEffects[i].ApplyStatEffect(this, name);
            }
        }

        canUsePotion = false;
        potionCooldown++;

        StartCoroutine(PotionCooldown());
        if (totalPotionCooldown != null) StopCoroutine(totalPotionCooldown);
        totalPotionCooldown = StartCoroutine(TotalPotionCooldown());

        if (onPotionUsed != null) onPotionUsed.Invoke();
        return true;
    }

    protected IEnumerator PotionCooldown()
    {
        yield return new WaitForSeconds(potionCooldown);
        canUsePotion = true;
    }

    protected IEnumerator TotalPotionCooldown()
    {
        while (potionCooldown > basePotionCooldown)
        {
            yield return new WaitForSeconds(30);
            potionCooldown--;
        }
        if (potionCooldown < basePotionCooldown) potionCooldown = basePotionCooldown;
    }
    #endregion

    #region - Stats -
    protected virtual void OnStatValueModified(Stat stat, int magnitude)
    {
        if (stat == statSheet.maxHealth)
        {
            currentHealth += magnitude;
            if (onHealthChange != null) onHealthChange.Invoke(magnitude);
        }
        else if (stat == statSheet.maxStamina)
        {
            currentStamina += magnitude;
            if (onStaminaChange != null) onStaminaChange.Invoke(magnitude);
        }
        else if (stat == statSheet.maxMana)
        {
            currentMana += magnitude;
            if (onManaChange != null) onManaChange.Invoke(magnitude);
        }
        else if (stat == statSheet.healthRegenRate)
        {
            if (healthRegenCoroutine != null) StopCoroutine(healthRegenCoroutine);
            healthRegenCoroutine = StartCoroutine(HealthRegeneration());
        }
        else if (stat == statSheet.staminaRegenRate)
        {
            if (staminaRegenCoroutine != null) StopCoroutine(staminaRegenCoroutine);
            staminaRegenCoroutine = StartCoroutine(StaminaRegeneration(0));
        }
        else if (stat == statSheet.manaRegenRate)
        {
            if (manaRegenCoroutine != null) StopCoroutine(manaRegenCoroutine);
            manaRegenCoroutine = StartCoroutine(ManaRegeneration(0));
        }
        else if (stat == statSheet.strength)
        {
            statSheet.carryCapacity.SetBaseValue(statSheet.strength.GetValue() * 3);
        }
        else if (stat == statSheet.armor) RecalculateArmorRating();
    }

    protected virtual void OnEquipmentChanged(InventoryEquipment newItem, InventoryEquipment oldItem)
    {
        if (newItem != null)
        {
            //var newEquip = newItem.item as Equipment;
            var newEquip = DatabaseManager.GetItem(newItem.itemID) as Equipment;
            for (int i = 0; i < newEquip.equipEffects.Length; i++)
            {
                newEquip.equipEffects[i].ApplyStatEffect(this, newEquip.name);
            }
            for (int i = 0; i < newItem.equipmentRunes.Count; i++)
            {
                if (newItem.equipmentRunes[i].runeActivation == RuneActivation.OnEquip)
                {
                    newItem.equipmentRunes[i].runeEffect.ApplyStatEffect(this, newEquip.name + " (Rune)");
                }
            }
            if (newEquip is Apparel) RecalculateArmorRating();
        }
        if (oldItem != null)// && oldItem.item != null)
        {
            //var oldEquip = oldItem.item as Equipment;
            var oldEquip = DatabaseManager.GetItem(oldItem.itemID) as Equipment;
            for (int i = 0; i < oldEquip.equipEffects.Length; i++)
            {
                oldEquip.equipEffects[i].RemoveStatEffect(this, oldEquip.name);
            }
            for (int i = 0; i < oldItem.equipmentRunes.Count; i++)
            {
                if (oldItem.equipmentRunes[i].runeActivation == RuneActivation.OnEquip)
                {
                    oldItem.equipmentRunes[i].runeEffect.RemoveStatEffect(this, oldEquip.name + " (Rune)");
                }
            }
            if (oldEquip is Apparel) RecalculateArmorRating();
        }
    }

    public void AddSpellEffect(SpellEffect effect, int statIndex, int magnitude, float duration, string sourceName)
    {
        statSheet.AddNewStatModifier(statIndex, magnitude);
        AddActiveEffect(effect, statIndex, magnitude, duration, sourceName);
    }

    //Removes a spell effect from stat and list
    public void RemoveSpellEffect(SpellEffect effect, Stat stat, int magnitude, string sourcename)
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            if (activeEffects[i].effect == effect && activeEffects[i].sourceName == sourcename)
            {
                RemoveActiveEffect(activeEffects[i]);
                break;
            }
        }
    }

    protected void AddActiveEffect(SpellEffect effect, int statIndex, int magnitude, float duration, string sourceName)
    {
        var newEffect = new ActiveEffect(statIndex, effect, magnitude, duration, sourceName);
        activeEffects.Add(newEffect);

        if (duration != 0) StartCoroutine(SpellEffectDuration(newEffect));
    }

    public void RemoveActiveEffect(ActiveEffect effect)
    {
        if (!activeEffects.Contains(effect)) Debug.Log("Effect not found");

        activeEffects.Remove(effect);
        statSheet.RemoveNewStatModifier(effect.statIndex, effect.magnitude);
        //statSheet.RemoveStatModifier(effect.stat, effect.magnitude);
        Debug.Log(effect.effect.name + " removed");
    }

    public IEnumerator SpellEffectDuration(ActiveEffect effect)
    {
        while (effect.duration > 0)
        {
            effect.duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        RemoveActiveEffect(effect);
    }
    #endregion
}

[System.Serializable]
public class ActiveEffect
{
    public int statIndex;
    public SpellEffect effect;
    public int magnitude;
    public float duration;
    public string sourceName;

    public ActiveEffect(int statIndex, SpellEffect effect, int magnitude, float duration, string sourceName)
    {
        this.statIndex = statIndex;
        this.effect = effect;
        this.magnitude = magnitude;
        this.duration = duration;
        this.sourceName = sourceName;
    }
}