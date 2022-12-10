using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public WeaponEquipSettings equipSettings;
    [HideInInspector] public CharacterCombat combat;
    [HideInInspector] public EquipmentSlot equipSlot;
    public Collider[] colliders;

    [Header("Weapon Properties")]
    public Weapon weapon;
    [SerializeField] public AudioSource audioSource;
    public ParticleSystem normalWeaponTrail;
    [HideInInspector] public bool runed;

    [Header("Weapon Poison")]
    public Poison poison;
    public int poisonCount;

    public List<EffectHolder> onStrikeEffects = new List<EffectHolder>();

    protected virtual void Start()
    {
        /*if (equipSettings == null) equipSettings = GetComponentInParent<WeaponEquipSettings>();
        if (equipSlot == EquipmentSlot.Primary_Off || equipSlot == EquipmentSlot.Secondary_Off)
        {
            transform.localPosition = weapon.secondaryPosition;
            transform.localRotation = Quaternion.Euler(weapon.secondaryRotation);
        }
        else transform.localRotation = Quaternion.Euler(weapon.primaryRotation);*/
    }

    //Trigger for striking an enemy
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)// && other.transform.name != combat.transform.name)
        {
            var target = other.GetComponentInParent<IDamageable>();
            if (target != null) combat.OnWeaponHit(this, other, target);
        }
    }

    public void OnAttackStart()
    {
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = true;
        SwingEffect();
    }

    public void OnAttackEnd()
    {
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
    }

    public void PlayImpactSound(CharacterStats stats, Collider other, DamageType type)
    {
        if (stats != null && stats.characterCombat != null)
        {
            //I may want to move this to the OnBlock method in CharacterStats
            if (stats.characterCombat.isBlocking)
            {
                if (stats.characterCombat.shield != null)
                {
                    if (stats.characterCombat.shield.isMetal)
                    {
                        audioSource.PlayOneShot(AudioHelper.instance.ShieldStruck(true));
                    }
                    else
                    {
                        audioSource.PlayOneShot(AudioHelper.instance.ShieldStruck(false));
                    }
                }
                else
                {
                    audioSource.PlayOneShot(AudioHelper.instance.ShieldStruck(true));
                }
            }
        }
        else
        {
            audioSource.clip = AudioHelper.instance.WeaponImpact(type);
            audioSource.Play();
        }
    }

    public void ApplyPoison(CharacterStats character)
    {
        if (character != null)
        {
            for (int i = 0; i < poison.potionEffects.Count; i++)
            {
                poison.potionEffects[i].ApplyStatEffect(character, poison.name);
            }
            poisonCount--;
            if (poisonCount <= 0) poison = null;
        }
    }

    public virtual void SwingEffect()
    {
        audioSource.clip = AudioHelper.instance.WeaponSwing();
        audioSource.Play();
        normalWeaponTrail.Play();
    }

    #region - Thrown Weapons -
    public virtual void RotateJavelin()
    {
        if (weapon is ThrownWeapon thrown && thrown.rotate == true)
        {
            var rot = transform.parent.transform.localRotation;
            transform.parent.transform.localRotation = Quaternion.Euler(rot.x, rot.y + 180, rot.z);
        }
    }

    public virtual void OnWeaponThrown()
    {
        if (weapon.weaponType != WeaponType.Thrown) return;
        if (poison != null)
        {
            poisonCount--;
            if (poisonCount <= 0) poison = null;
        }
        gameObject.SetActive(false);
    }

    public virtual void OnThrownReload()
    {
        if (weapon.weaponType != WeaponType.Thrown) return;
        gameObject.SetActive(true);
        equipSettings.WeaponAlignment();
    }
    #endregion
}