using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    public delegate void OnWeaponsDrawn(bool weaponsDrawn);
    public OnWeaponsDrawn onWeaponsDrawn;

    [HideInInspector] public EquipmentManager equipManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public Animator animator;
    public CharacterStats characterStats;

    [Header("Unarmed")]
    public Weapon unarmed;
    public Transform strikingPoint;
    public float strikeRadius = 0.3f;

    [HideInInspector] public bool canDamagePrimary = false;
    [HideInInspector] public bool canDamageSecondary = false;
    [HideInInspector] public bool powerAttack = false;
    [HideInInspector] public AttackDirection lastAttackDir;
    //[HideInInspector] 
    public bool weaponsDrawn = false;
    [HideInInspector] public bool isBlocking = false;

    [HideInInspector] public bool characterBowDrawn = false;
    [HideInInspector] public bool castingSpell = false;
    [HideInInspector] public List<IDamageable> targetsToIgnore = new List<IDamageable>();
    [Space]

    [HideInInspector] public Shield shield;
    public bool canParry { get; protected set; }
    public float parryWindow = 0.5f;
    protected Coroutine parryCoroutine;
    [Space]
    [HideInInspector] public bool hasTwoHander = false;
    [HideInInspector] public bool hasBow = false;
    [HideInInspector] public BowAnimHelper bowAnim;
    public bool projectileReturn;

    public virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterStats = GetComponent<CharacterStats>();
        characterController = GetComponent<CharacterController>();
        equipManager = GetComponent<EquipmentManager>();
    }

    public virtual void OnUnarmedHit(Collider other, IDamageable damageable)
    {
        var character = other.GetComponentInParent<CharacterStats>();
        if (character != null)
        {
            if (NoFriendlyFire(character)) return;
            if (character.characterCombat.canParry)
            {
                OnStagger();
                audioSource.PlayOneShot(AudioHelper.instance.WeaponImpact(DamageType.Bludgeoning));
                AddCameraShake(character);
                return;
            }
        }

        WeaponDamage damage = GetWeaponDamage(unarmed);
        damageable.ApplyDamage(characterStats, damage.magnitude, damage.type, unarmed.isLethal);

        audioSource.PlayOneShot(AudioHelper.instance.UnarmedStrike());
        AddCameraShake(character);
    }

    public virtual void OnWeaponHit(ActiveWeapon activeWeapon, Collider collider, IDamageable damageable)
    {
        if (targetsToIgnore.Contains(damageable)) return;
        targetsToIgnore.Add(damageable);

        var character = collider.GetComponentInParent<CharacterStats>();
        if (character != null)
        {
            if (NoFriendlyFire(character)) return;
            if (character.characterCombat.canParry)
            {
                OnStagger();
                activeWeapon.PlayImpactSound(character, collider, DamageType.Bludgeoning);
                AddCameraShake(character);
                return;
            }
        }
        if (activeWeapon.poison != null) activeWeapon.ApplyPoison(character);

        WeaponDamage damage = GetWeaponDamage(activeWeapon.weapon);
        damageable.ApplyDamage(characterStats, damage.magnitude, damage.type, activeWeapon.weapon.isLethal);
        StartCoroutine(characterStats.HitFreeze());

        activeWeapon.PlayImpactSound(character, collider, damage.type);
        AddCameraShake(character);
    }

    protected virtual bool NoFriendlyFire(CharacterStats character)
    {
        if (character.characterController.team == null) return false; //No team
        else if (character.characterController.team == characterController.team) return true; //Same team
        else if (characterController.team.alliedTeams.Contains(character.characterController.team)) return true; //Ally team
        return false; //different team
    }

    protected virtual void AddCameraShake(CharacterStats stats)
    {
        CameraHelper.instance.CameraShake(3f, 0.1f);
    }

    public virtual AttackDirection GetAttackDirection(Weapon weapon)
    {
        return weapon.defaultAttack;
    }
    
    public virtual WeaponDamage GetWeaponDamage(Weapon weapon)
    {
        float damage = 0;
        var type = DamageType.Bludgeoning;

        if (lastAttackDir == AttackDirection.Bash)
        {
            damage = Random.Range(weapon.minBashDmg, weapon.maxBashDmg);
            type = weapon.bashDamage;
        }
        else if (lastAttackDir == AttackDirection.Slash)
        {
            damage = Random.Range(weapon.minSlashDmg, weapon.maxSlashDmg);
            type = weapon.slashDamage;
        }
        else
        {
            damage = Random.Range(weapon.minThrustDmg, weapon.maxThrustDmg);
            type = weapon.thrustDamage;
        }
        
        switch (weapon.weaponSkill)
        {
            case WeaponSkill.Axe:
                {
                    damage += characterStats.statSheet.axes.GetStatDecim();
                    break;
                }
            case WeaponSkill.Blade:
                {
                    damage += characterStats.statSheet.blades.GetStatDecim();
                    break;
                }
            case WeaponSkill.Club:
                {
                    damage += characterStats.statSheet.clubs.GetStatDecim();
                    break;
                }
            case WeaponSkill.Polearm:
                {
                    damage += characterStats.statSheet.polearms.GetStatDecim();
                    break;
                }
            case WeaponSkill.Shield:
                {
                    damage += characterStats.statSheet.shields.GetStatDecim();
                    break;
                }
            case WeaponSkill.Striker:
                {
                    damage += characterStats.statSheet.striker.GetStatDecim();
                    break;
                }
            case WeaponSkill.Ranged:
                {
                    Debug.LogWarning("This should not come back");
                    damage += characterStats.statSheet.ranged.GetStatDecim();
                    break;
                }
        }
        
        if (weapon.isFinesseWeapon) damage += characterStats.statSheet.finesse.GetStatDecim();
        else damage += characterStats.statSheet.strength.GetStatDecim();
        
        if (powerAttack == true) damage *= 2;

        damage = CriticalHit(weapon, damage);
        
        return new WeaponDamage(damage, type);
    }

    public virtual WeaponDamage GetProjectileDamage(Weapon weapon)
    {
        DamageType type = DamageType.Piercing;
        float damage = Random.Range(weapon.minThrustDmg, weapon.maxThrustDmg);

        if (weapon.weaponSkill == WeaponSkill.Axe)
        {
            damage = Random.Range(weapon.minSlashDmg, weapon.maxSlashDmg);
            type = DamageType.Slashing;
        }
        else if (weapon.weaponSkill == WeaponSkill.Club)
        {
            damage = Random.Range(weapon.minBashDmg, weapon.maxBashDmg);
            type = DamageType.Bludgeoning;
        }
        
        damage += characterStats.statSheet.ranged.GetStatDecim();
        
        if (weapon.isFinesseWeapon) damage += characterStats.statSheet.finesse.GetStatDecim(); //Daggers, Darts, Arrows
        else damage += characterStats.statSheet.strength.GetStatDecim(); //Axes, Hammers, Javelins
        
        damage = CriticalHit(weapon, damage);
        
        return new WeaponDamage(damage, type);
    }

    public virtual void FireProjectile(ActiveWeapon activeWeapon)
    {
        //Meant to be overwritten
    }

    public virtual float CriticalHit(Weapon weapon, float initial)
    {
        float critRoll = Random.Range(1f, 100f);
        if (critRoll <= (weapon.critChance + characterStats.statSheet.guile.GetValue()))
            return initial *= 3;
        else return initial;
    }

    public virtual void ClearTargetList()
    {
        targetsToIgnore.Clear();
        targetsToIgnore.Add(characterStats);
    }

    public virtual void ReadyWeapons()
    {
        if (equipManager.mainHand == null && equipManager.offHand == null)
        {
            weaponsDrawn = !weaponsDrawn;
            if (equipManager.canEquipWeapons) animator.SetBool("weaponsOut", weaponsDrawn);
            if (onWeaponsDrawn != null) onWeaponsDrawn.Invoke(weaponsDrawn);
        }
        else
        {
            if (equipManager.canEquipWeapons) animator.SetTrigger("sheathe");
            if (hasBow == true && characterBowDrawn == true) bowAnim.SetDrawWeight(0);
        }
    }

    protected virtual void OnStartBlock()
    {
        if (parryCoroutine != null) StopCoroutine(parryCoroutine);
        parryCoroutine = StartCoroutine(ParryTime());
    }

    protected IEnumerator ParryTime()
    {
        canParry = true;
        yield return new WaitForSeconds(parryWindow);
        canParry = false;
    }

    protected virtual void OnStagger()
    {
        animator.Play("staggered", 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (strikingPoint != null)
        {
            Gizmos.DrawWireSphere(strikingPoint.position, strikeRadius);
        }
    }
}

public struct WeaponDamage
{
    public float magnitude;
    public DamageType type;

    public WeaponDamage(float magnitude, DamageType type)
    {
        this.magnitude = magnitude;
        this.type = type;
    }
}