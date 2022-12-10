using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipSettings : MonoBehaviour
{
    [SerializeField] private ItemPickup pickup;
    [SerializeField] private ActiveWeapon activeWeapon;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider[] colliders;

    [HideInInspector] public CharacterStats characterStats;
    [HideInInspector] public CharacterCombat characterCombat;
    [HideInInspector] public EquipmentManager equipManager;
    public Weapon weapon;

    private void Start()
    {
        weapon = activeWeapon.weapon;
        if (characterStats != null)
        {
            activeWeapon.combat = characterCombat;
            WeaponAlignment();
            SetPhysics(true);
            characterStats.onDeath += DisableWeapon;
        }
        else
        {
            SetPhysics(false);
            activeWeapon.enabled = false;
            this.enabled = false;
        }
    }

    private void SetPhysics(bool isEquipped)
    {
        foreach (Collider coll in colliders)
        {
            coll.enabled = !isEquipped;
            coll.isTrigger = isEquipped;
            if (weapon is Arrow) coll.enabled = false;
        }
        rb.isKinematic = isEquipped;
        rb.useGravity = !isEquipped;
        pickup.enabled = !isEquipped;
    }

    public void WeaponAlignment()
    {
        if (weapon is Arrow)
        {
            PlaceOnBack();
            return;
        }

        bool placeOnBack = false;
        int num = (int)activeWeapon.equipSlot;

        if (num == 1 || num == 3)
        {
            transform.localPosition = weapon.secondaryPosition;
            transform.localRotation = Quaternion.Euler(weapon.secondaryRotation);
        }

        if (characterCombat.weaponsDrawn == false) placeOnBack = true;
        else
        {
            //Character has their secondary weapons drawn
            if (equipManager.usingSecondaryWeaponSet)
            {
                //this weapon is equipped in a primary slot
                if (num == 0 || num == 1) placeOnBack = true;
            }
            else //Character has their primary weapons drawn
            {
                if (num == 2 || num == 3) placeOnBack = true;
            }
        }

        //Weapons are sheathed or this is equipped in a secondary slot, place it on the back
        if (placeOnBack == true) PlaceOnBack();
        else //Weapons are drawn and this weapon is in the currently used set
        {
            //if (activeWeapon.equipSlot == EquipmentSlot.Primary_Off || activeWeapon.equipSlot == EquipmentSlot.Secondary_Off || activeWeapon.weapon.weaponType == WeaponType.Bow)
            if (num == 1 || num == 3 || weapon.weaponType == WeaponType.Bow) PlaceInHand(false);
            else PlaceInHand(true);
        }
    }

    public void PlaceOnBack()
    {
        Transform target = equipManager.weaponsBack;
        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.parent = target;
    }

    public void PlaceInHand(bool mainHand)
    {
        Transform target = equipManager.weapon_R;
        if (!mainHand) target = equipManager.weapon_L;
        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.parent = target;
    }

    //Call this function when the wielder dies
    public void DisableWeapon()
    {
        SetPhysics(false);
        activeWeapon.enabled = false;
        this.enabled = false;
    }

    #region - Context Menu -
    [ContextMenu("Assign Components")]
    private void AssignComponents()
    {
        pickup = GetComponent<ItemPickup>();
        activeWeapon = GetComponentInChildren<ActiveWeapon>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        weapon = activeWeapon.weapon;
        activeWeapon.colliders = colliders;
        activeWeapon.equipSettings = this;
        activeWeapon.audioSource = GetComponentInChildren<AudioSource>();
        activeWeapon.normalWeaponTrail = GetComponentInChildren<ParticleSystem>();
    }
    #endregion
}
