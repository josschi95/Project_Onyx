using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEquipmentManager : EquipmentManager
{
    [Header("NPC Specific Values")]
    public EquipOptions equipmentOptions;
    [Space]
    [SerializeField] NPCCombat combat;
    [SerializeField] Animator anim;
    public bool equipOnStart = false;
    [Space]
    [HideInInspector] public bool hasTwoSets = false;

    public Apparel[] defaultApparel;
    public Weapon[] defaultWeapons;

    protected override void Start()
    {
        base.Start();
        combat.weaponsDrawn = equipOnStart;
        if (canEquipWeapons) anim.SetBool("weaponsOut", equipOnStart);

        StartCoroutine(EquipDelay());
        onEquipmentChanged += delegate { HasRangedWeapons(); };
        if (equipmentOptions == EquipOptions.All)
            skinColor = baseMeshes[0].GetComponent<SkinnedMeshRenderer>().material.GetColor("_Color_Skin");
    }

    protected IEnumerator EquipDelay()
    {
        //Let other scripts do their thing
        yield return new WaitForSeconds(0.1f);
        EquipDefaults();
    }

    protected virtual void EquipDefaults()
    {
        if (canEquipApparel == false && canEquipWeapons == false) return;

        for (int i = 0; i < defaultApparel.Length; i++)
        {
            Equip(new InventoryEquipment(defaultApparel[i].itemID, 1, true, false), (int)defaultApparel[i].equipSlot);
        }

        for (int i = 0; i < defaultWeapons.Length; i++)
        {
            if (defaultWeapons[i] == null) continue;
            if (i > 1)
            {
                usingSecondaryWeaponSet = true;
                hasTwoSets = true;
            }
            Equip(new InventoryEquipment(defaultWeapons[i].itemID, 1, true, false), i);
        }

        SwapWeaponSets(false, true);
    }

    public void HasRangedWeapons()
    {
        combat.hasRanged = false;
        for (int i = 0; i < currentWeapons.Length; i++)
        {
            if (currentWeapons[i] != null)
                if (currentWeapons[i].weapon.weaponType == WeaponType.Bow || currentWeapons[i].weapon.weaponType == WeaponType.Thrown)
                    combat.hasRanged = true;
        }
    }
}
public enum EquipOptions { All, WeaponsOnly, JewelryOnly, WeaponsAndJewelry, None }