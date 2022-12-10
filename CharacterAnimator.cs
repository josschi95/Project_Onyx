using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Animator anim;
    [SerializeField] protected CharacterCombat combat;
    public AnimationClip taunt;
    protected EquipmentManager equipManager;
    protected AnimatorOverrideController overrideController;

    [Space]
    public AnimatorOverrideController unarmedSet; //No weapons
    public AnimatorOverrideController oneHandRight; //1H weapon in right hand only
    public AnimatorOverrideController oneHandLeft; //1H weapon in left hand only
    public AnimatorOverrideController dualWield; //1H weapon in both hands
    public AnimatorOverrideController heavySet; //Heavy weapon
    public AnimatorOverrideController poleSet; //Pole weapon
    public AnimatorOverrideController bowSet; //Bow

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        overrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = overrideController;
        equipManager = GetComponent<EquipmentManager>();
        equipManager.onWeaponSetChange += delegate { UpdateAnimSet(); };
        combat.onWeaponsDrawn += delegate { UpdateAnimSet(); };
        overrideController["taunt"] = taunt;
    }

    //Changes attack animations based on weapon
    public virtual void UpdateAnimSet()
    {
        if (equipManager.canEquipWeapons == false) return;

        if (equipManager.mainHand == null && equipManager.offHand == null)
        {
            SetUnarmed();
        }
        else if (combat.weaponsDrawn == false)
        {
            SetUnarmed();
        }
        else
        {
            Weapon weaponRight = null;
            Weapon weaponLeft = null;

            if (equipManager.mainHand != null)
            {
                anim.SetLayerWeight(2, 1);
                anim.SetLayerWeight(3, 1);
                weaponRight = equipManager.mainHand.weapon;
                if (weaponRight.weaponType == WeaponType.Heavy)
                {
                    overrideController = heavySet;
                }
                else if (weaponRight.weaponType == WeaponType.Pole)
                {
                    overrideController = poleSet;
                }
                else if (weaponRight.weaponType == WeaponType.Bow)
                {
                    anim.SetLayerWeight(2, 0);
                    overrideController = bowSet;
                    combat.bowAnim = equipManager.mainHand.GetComponent<BowAnimHelper>();
                    combat.bowAnim.anim.SetFloat("weaponSpeed1", weaponRight.weaponSpeed);
                }
                else
                {
                    //This is being set in case there's not a weapon in the left hand
                    anim.SetLayerWeight(3, 0);
                    overrideController = oneHandRight;
                }

                anim.SetFloat("weaponSpeed1", weaponRight.weaponSpeed);
                anim.SetInteger("weaponMain", (int)weaponRight.weaponType);
            }

            if (equipManager.offHand != null)
            {
                weaponLeft = equipManager.offHand.weapon;
                if (weaponLeft is Arrow) return;
                anim.SetLayerWeight(3, 1);
                if (weaponLeft.weaponType != WeaponType.Shield)
                {
                    //Dual wielding
                    if (equipManager.mainHand != null)
                    {
                        overrideController = dualWield;
                        anim.SetFloat("weaponSpeed2", weaponLeft.weaponSpeed);
                        anim.SetInteger("weaponOff", (int)weaponLeft.weaponType);
                    }
                    else //There is no weapon in the right hand
                    {
                        overrideController = oneHandLeft;
                        anim.SetFloat("weaponSpeed2", weaponLeft.weaponSpeed);
                        anim.SetInteger("weaponOff", (int)weaponLeft.weaponType);
                    }
                }
            }
        }
        overrideController["taunt"] = taunt;
        anim.runtimeAnimatorController = overrideController;
    }

    protected virtual void SetUnarmed()
    {
        overrideController = unarmedSet;
        anim.SetLayerWeight(2, 0);
        anim.SetLayerWeight(3, 0);
    }

    //Use this if I keep lower body in player
    protected virtual void RightHandWeight(int weight)
    {
        anim.SetLayerWeight(3, weight);
    }

    protected virtual void LeftHandWeight(int weight)
    {
        anim.SetLayerWeight(4, weight);
    }

    public virtual void OnAbilityChange(Ability ability)
    {
        overrideController["ability"] = ability.animClip;
    }
}
