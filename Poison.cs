using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Poison", menuName = "Inventory/Potions/Poison")]

public class Poison : Potion
{
    [Header("Poison Properties")]
    public PoisonType poisonType;
    [SerializeField] private bool damageOverTime;
    public bool instantEffect = false;
    public int uses;

    public void PlayerPoisonOptions(CharacterCombat combat)
    {
        bool rightHand = false;
        bool leftHand = false;
        var equip = combat.equipManager;
        if (equip.mainHand != null && WeaponCanBePoisoned(equip.mainHand)) rightHand = true;
        if (equip.offHand != null && WeaponCanBePoisoned(equip.offHand)) leftHand = true;

        if (rightHand == false && leftHand == false)
        {
            UIManager.instance.DisplayPopup("No Weapon to Poison");
            return;
        }
        else
        {
            SelectionPanel panel = UIManager.instance.selectionPanel;

            panel.gameObject.SetActive(true);


            panel.button00.onClick.AddListener(delegate
            {
                panel.gameObject.SetActive(false);
                return;
            });

            //Both weapons can be poisoned
            if (rightHand == true && leftHand == true)
            {
                panel.DisplayOptions("Cancel", "Poison " + equip.mainHand.weapon.name + " (Right)", "Poison " + equip.offHand.weapon.name + " (Left)", "", "");
                panel.header.text = "Poison Weapon";
                panel.description.text = "Apply " + this.name + " to weapon?";
                panel.button01.onClick.AddListener(delegate
                {
                    ApplyWeaponPoison(equip.mainHand);
                    panel.gameObject.SetActive(false);
                });

                panel.button02.onClick.AddListener(delegate
                {
                    ApplyWeaponPoison(equip.offHand);
                    panel.gameObject.SetActive(false);
                });
            }
            else
            {
                //Only right weapon
                if (rightHand == true && leftHand == false)
                {
                    panel.DisplayOptions("Cancel", "Poison " + equip.mainHand.weapon.name + " (Right)", "", "", "");
                    panel.header.text = "Poison Weapon";
                    panel.description.text = "Apply " + this.name + " to weapon?";
                    panel.button01.onClick.AddListener(delegate
                    {
                        ApplyWeaponPoison(equip.mainHand);
                        panel.gameObject.SetActive(false);
                    });
                }

                //Only left weapon
                else if (rightHand == false && leftHand == true)
                {
                    panel.DisplayOptions("Cancel", "Poison " + equip.offHand.weapon.name + " (Right)", "", "", "");
                    panel.header.text = "Poison Weapon";
                    panel.description.text = "Apply " + this.name + " to weapon?";
                    panel.button01.onClick.AddListener(delegate
                    {
                        ApplyWeaponPoison(equip.offHand);
                        panel.gameObject.SetActive(false);
                    });
                }
            }
        }
    }

    public bool WeaponCanBePoisoned(ActiveWeapon activeWeapon)
    {
        if (activeWeapon.weapon.canBePoisoned == false || activeWeapon.poison != null)
        {
            return false;
        }
        return true;
    }

    public void ApplyWeaponPoison(ActiveWeapon weapon)
    {
        if (WeaponCanBePoisoned(weapon) == false) return;

        weapon.poison = this;

        int count = uses;
        //if (PlayerSkills.instance.playerPerks.Contains(PlayerSkills.instance.perkManager.DebilitatingPoisons) count *= 2;
        weapon.poisonCount = count;
        RemoveFromInventory();
    }
}

//I don't know if I'll actually be able to implement this
public enum PoisonType { Ingested, Injury, Contact }