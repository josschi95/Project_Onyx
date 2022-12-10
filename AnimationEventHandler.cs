using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private CharacterSpellcasting spellcasting;
    private CharacterController controller;
    private EquipmentManager equipManager;
    private AudioHelper audioHelper;
    private CharacterCombat combat;
    private CharacterStats stats;
    private Animator anim;

    public GameObject basicArrow;

    private void Start()
    {
        spellcasting = GetComponentInParent<CharacterSpellcasting>();
        controller = GetComponentInParent<CharacterController>();
        equipManager = GetComponentInParent<EquipmentManager>();
        combat = GetComponentInParent<CharacterCombat>();
        stats = GetComponentInParent<CharacterStats>();
        audioHelper = AudioHelper.instance;
        anim = GetComponent<Animator>();
    }

    #region - Melee Attacks -
    //enables colliders on weapons
    public void StartAttack_Primary()
    {
        ActiveWeapon activeWeapon = equipManager.mainHand;
        if (activeWeapon != null) activeWeapon.OnAttackStart();
    }

    public void StartAttack_Secondary()
    {
        ActiveWeapon activeWeapon = equipManager.offHand;
        if (activeWeapon != null) activeWeapon.OnAttackStart();
    }

    //disables colliders on weapons
    public void EndAttack()
    {
        ActiveWeapon mainHand = equipManager.mainHand;
        if (mainHand != null) mainHand.OnAttackEnd();

        ActiveWeapon offHand = equipManager.offHand;
        if (offHand != null) offHand.OnAttackEnd();

        combat.canDamagePrimary = false;
        combat.canDamageSecondary = false;
        combat.powerAttack = false;
    }

    public void UnarmedStrike()
    {
        Collider[] targets = Physics.OverlapSphere(combat.strikingPoint.position, combat.strikeRadius);
        foreach (Collider other in targets)
        {
            if (other.isTrigger == false && other.transform.name != combat.transform.name)
            {
                var target = other.GetComponentInParent<IDamageable>();
                if (target != null) combat.OnUnarmedHit(other, target);
            }
        }
    }

    public void OnPowerAttack()
    {
        combat.powerAttack = true;
        stats.SpendStamina(20);
        Debug.Log("This is a placeholder");
        //Later to be replaced with some kind of weapon.weight / statsheet.might.GetValue() kinda thing
    }
    #endregion

    #region - Input Controls -
    //Does not allow character to move
    public void NoInput()
    {
        controller.acceptInput = false;
    }

    //Returns to normal character controls
    public void YesInput()
    {
        controller.acceptInput = true;
    }

    public void StopMovement()
    {
        controller.canMove = false;
    }
    #endregion

    public void TriggerAbility()
    {
        stats.currentAbility.Use(stats);
    }

    public void FireSpellProjectile()
    {
        spellcasting.CastReadiedSpell();
    }

    public void FireProjectile()
    {
        if (combat.hasBow && equipManager.projectilesOff != null)
            combat.FireProjectile(equipManager.offHand);

        else if (equipManager.projectilesMain != null)
            combat.FireProjectile(equipManager.mainHand);
    }

    public void FireOffhandProjectile()
    {
        if (equipManager.projectilesOff != null)
            combat.FireProjectile(equipManager.offHand);
    }

    #region - Bows -

    public void OnBeginDraw()
    {
        combat.bowAnim.DrawArrow();
        StartCoroutine(SetBowWeight());
    }

    private IEnumerator SetBowWeight()
    {
        while (combat.animator.GetCurrentAnimatorStateInfo(2).IsName("bow_load"))
        {
            float weight = combat.animator.GetCurrentAnimatorStateInfo(2).normalizedTime;
            combat.bowAnim.SetDrawWeight(weight);
            yield return null;
        }
    }

    public void ShowDrawnArrow()
    {
        basicArrow.SetActive(true);
        combat.audioSource.PlayOneShot(audioHelper.takeArrow);
    }

    public void HideDrawnArrow()
    {
        basicArrow.SetActive(false);
    }
    #endregion

    #region - Thrown Weapons -
    public void RotateJav_Right()
    {
        if (equipManager.mainHand != null) equipManager.mainHand.RotateJavelin();
    }

    public void RotateJav_Left()
    {
        if (equipManager.offHand != null) equipManager.offHand.RotateJavelin();
    }

    public void HideThrownRight()
    {
        if (equipManager.mainHand != null) equipManager.mainHand.OnWeaponThrown();
    }

    public void HideThrownLeft()
    {
        if (equipManager.offHand != null) equipManager.offHand.OnWeaponThrown();
    }

    public void ReloadRight()
    {
        if (equipManager.mainHand != null) equipManager.mainHand.OnThrownReload();
    }

    public void ReloadLeft()
    {
        if (equipManager.offHand != null) equipManager.offHand.OnThrownReload();
    }
    #endregion

    //To be used in the future to play footstep sounds
    public void OnStep()
    {
        //Debug.Log("Step");
        //ObjectPooler.SpawnFromPool_Static("magicStep", transform.position, Quaternion.identity);
    }

    //Sheath or Draw weapons
    public void SheatheWeapons()
    {
        //Drawing weapons
        if (combat.weaponsDrawn == false)
        {
            combat.weaponsDrawn = true;
            anim.SetBool("weaponsOut", true);
        }
        //Sheathing weapons
        else
        {
            combat.weaponsDrawn = false;
            anim.SetBool("weaponsOut", false);
        }

        for (int i = 0; i < equipManager.currentWeapons.Length; i++)
        {
            if (equipManager.currentWeapons[i] != null && equipManager.currentWeapons[i].equipSettings != null)
                equipManager.currentWeapons[i].equipSettings.WeaponAlignment();
        }
        if (combat.onWeaponsDrawn != null) combat.onWeaponsDrawn.Invoke(combat.weaponsDrawn);
    }

    //Swap between primary and secondary weapon set
    public void SwapWeapons()
    {
        for (int i = 0; i < equipManager.currentWeapons.Length; i++)
        {
            if (equipManager.currentWeapons[i] != null && equipManager.currentWeapons[i].equipSettings != null)
                equipManager.currentWeapons[i].equipSettings.WeaponAlignment();
        }
    }
}
