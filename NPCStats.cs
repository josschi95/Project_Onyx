using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStats : CharacterStats, IDamageable
{
    //Use this to report a crime if the character is not hostile
    //Could use this to tie into some perks as well
    public delegate void OnFirstStrike();
    public OnFirstStrike onFirstStrike;

    [Header("NPC Stats")]
    private NPCController NPCController;
    public CreatureType creatureType;
    public int maxrecoilResistance = 10;
    private float currentRecoilResistance;

    public bool isStunned = false;
    float stunTime = 2f;
    [SerializeField] int expValue;

    protected override void Start()
    {
        base.Start();
        NPCController = GetComponent<NPCController>();
        currentRecoilResistance = maxrecoilResistance;

        //Instantiate copy of base stat sheet
        var newSheet = ScriptableObject.Instantiate(statSheet);
        statSheet = newSheet;
        statSheet.CompileAllStats();
    }

    public override void ApplyDamage(CharacterStats attacker, float damage, DamageType damageType, bool isLethal)
    {
        if (isDead) return;

        //Could run into an issue if I don't also set a bool, if the NPC restores their HP to full
        if (currentHealth == statSheet.maxHealth.GetValue()) if (onFirstStrike != null) onFirstStrike.Invoke();

        //Double damage from undetected attackers. Currently this stacks with critical hits
        if (attacker != null) //Should only be true for spells
        {
            if (NPCController.charDetect.detectedCharacters.Contains(attacker.characterController) == false) damage *= 2;
        }

        base.ApplyDamage(attacker, damage, damageType, isLethal);
        if (NPCController.debugging) Debug.Log(transform.name + " takes " + damage + " " + damageType.ToString() + " damage");
        if (isDead) return;

        NPCController.StopAllCoroutines();

        //Once I make the change, this will only be applied from ApplyWeaponDamage
        currentRecoilResistance -= damage;
        if (currentRecoilResistance <= 0)
        {
            animator.Play("damage");
            currentRecoilResistance = maxrecoilResistance;
        }
    }

    protected override void BloodParticles(DamageType damageType)
    {
        if (creatureType == CreatureType.Undead || creatureType == CreatureType.Slime) return;
        base.BloodParticles(damageType);
    }

    public override void Die()
    {
        PlayerManager.instance.GainEXP(expValue);
        base.Die();
    }

    public override void Unconscious()
    {
        PlayerManager.instance.GainEXP(expValue);
        base.Unconscious();
    }

    IEnumerator Stunned()
    {
        isStunned = true;

        yield return new WaitForSeconds(stunTime);

        isStunned = false;
    }
}

public enum CreatureType { Human, Beast, Undead, Demon, Giant, Slime }