using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCombat : CharacterCombat
{
    [Header("NPC Combat")]
    [SerializeField] AICharacterDetection detection;
    [SerializeField] NPCController controller;

    #region - Attacks and Parameters
    public NPCAttackAction[] attackActions;
    [Space]
    //[HideInInspector] 
    public float attackRange; //maximum range of current weapons
    public float maxRange;

    [HideInInspector] public float primaryRange; //minimum range of primary weapon set

    [HideInInspector] public float secondaryRange; //minimum range of secondary weapon set
    [Space]
    #endregion

    [HideInInspector] public bool hasToken;
    [HideInInspector] public bool hasRanged = false;
    [HideInInspector] public bool dualWielding = false;

    //[HideInInspector] 
    public bool recoveringFromAttack = false;
    [HideInInspector] public Coroutine recoveryRoutine;
    public NPCAttackAction currentAttack;

    public override void Start()
    {
        base.Start();
        equipManager.onWeaponSetChange += UpdateCurrentAttackRange;
        if (equipManager.canEquipWeapons == false) MonsterAttackRange();
    }

    protected override void AddCameraShake(CharacterStats stats)
    {
        if (stats != null && stats is PlayerStats)
            base.AddCameraShake(stats);
    }

    public void UpdateCurrentAttackRange(bool usingSecondary)
    {
        if (equipManager.canEquipWeapons == false)
        {
            MonsterAttackRange();
            return;
        }

        //Primary Weapons
        if (equipManager.currentWeapons[0] != null)
            primaryRange = equipManager.currentWeapons[0].weapon.reach;

        if (equipManager.currentWeapons[1] != null && equipManager.currentWeapons[1].weapon.reach > primaryRange)
            primaryRange = equipManager.currentWeapons[1].weapon.reach;

        //Secondary Weapons
        if (equipManager.currentWeapons[2] != null)
            secondaryRange = equipManager.currentWeapons[2].weapon.reach;

        if (equipManager.currentWeapons[3] != null && equipManager.currentWeapons[3].weapon.reach > secondaryRange)
            secondaryRange = equipManager.currentWeapons[3].weapon.reach;

        if (usingSecondary) attackRange = secondaryRange;
        else attackRange = primaryRange;

        if (primaryRange > secondaryRange) maxRange = primaryRange;
        else maxRange = secondaryRange;

        controller.log.AddEntry("using secondary " + usingSecondary);
        controller.log.AddEntry("range: " + attackRange);
    }

    protected void MonsterAttackRange()
    {
        for (int i = 0; i < attackActions.Length; i++)
        {
            if (attackActions[i].maxRange > attackRange)
                attackRange = attackActions[i].maxRange;
        }
        maxRange = attackRange;
    }

    public override void FireProjectile(ActiveWeapon activeWeapon)
    {
        //Aiming
        Vector3 target = controller.target.center.position;
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Instantiate
        GameObject newProjectile = null;
        if (activeWeapon.weapon is ThrownWeapon thrown)
        {
            newProjectile = Instantiate(thrown.projectile, gameObject.transform.position, lookRotation);
        }
        else if (activeWeapon.weapon is Arrow arrow)
        {
            GameObject bowArrow = equipManager.offHand.GetComponentInChildren<BowAnimHelper>().arrow;
            newProjectile = Instantiate(arrow.arrowPrefab, bowArrow.transform.position, lookRotation);
        }
        else Debug.LogWarning("Not instantiating an arrow or thrown weapon");
        newProjectile.transform.LookAt(target);

        Projectile projectileStats = newProjectile.GetComponentInChildren<Projectile>();
        projectileStats.OnSpawn(this, activeWeapon.weapon, activeWeapon.poison, projectileReturn);
    }

    public void SetNextAttack(NPCAttackAction attack)
    {
        currentAttack = attack;
        if (attack != null) attackRange = attack.maxRange;
        else attackRange = maxRange;
    }

    #region - Attacking -
    //Set trigger for a single attack
    public void SingleAttack(string trigger, float cooldown = 2)
    {
        //default attack1, attack2
        animator.SetTrigger(trigger);
        if (recoveryRoutine != null) StopCoroutine(recoveryRoutine);
        recoveryRoutine = StartCoroutine(AttackRecovery(cooldown));
    }

    public void PowerAttack(string trigger, float cooldown = 4)
    {
        animator.SetTrigger("powerAttack");
        animator.SetTrigger(trigger);
        if (recoveryRoutine != null) StopCoroutine(recoveryRoutine);
        recoveryRoutine = StartCoroutine(AttackRecovery(cooldown));
    }

    //Perform the same attack repeatedly
    public void ComboAttack(string trigger, int num)
    {
        StartCoroutine(Combo(trigger, num));
    }

    //Wait until the trigger can be set again, and then set it
    private IEnumerator Combo(string trigger, int num)
    {
        recoveringFromAttack = true;
        float t = 2 * num;

        while (num > 0)
        {
            if (animator.GetBool(trigger) == false)
            {
                ClearTargetList();
                animator.SetTrigger(trigger);
                num--;
            }
            yield return null;
        }
        if (recoveryRoutine != null) StopCoroutine(recoveryRoutine);
        recoveryRoutine = StartCoroutine(AttackRecovery(t));
    }

    //Set bool true for holdPrimary/holdSecondary
    public void RangedAttack(bool primary = true)
    {
        if (primary) animator.SetBool("holdPrimary", true);
        else animator.SetBool("holdSecondary", true);
        StartCoroutine(RangedDelay());
    }

    //Wait 1 second for the animator to run
    private IEnumerator RangedDelay(bool primary = true)
    {
        yield return new WaitForSeconds(1);

        if (primary) animator.SetBool("holdPrimary", false);
        else animator.SetBool("holdSecondary", false);

        float t = 3;
        if (recoveryRoutine != null) StopCoroutine(recoveryRoutine);
        recoveryRoutine = StartCoroutine(AttackRecovery(t));
    }

    public IEnumerator AttackRecovery(float recoveryTime)
    {
        recoveringFromAttack = true;
        currentAttack = null;
        yield return new WaitForSeconds(recoveryTime);
        recoveringFromAttack = false;
    }
    #endregion
}
public enum CombatBehavior { Charge, Impatient, Patient, Reserved }

public enum CombatOptions { OnlyMelee, OnlyRange, MeleeFocus, RangeFocus, MagicFocus }

/* Charge: immediately engage target on sight
 * Impatient: will wait up to x seconds before engaging
 * Patient: wait until target enters attack range
 * Reserved: will only attack if attacked */