using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PerkManager", menuName = "Perks/Perk Manager")]
public class PerkManager : ScriptableObject
{
    public Perk[] masterPerkList;

    [Header("Background Perks")]
    public Perk agent;
    public Perk artisan;
    public Perk cityGuard;
    public Perk commoner;
    public Perk criminal;
    public Perk hero;
    public Perk hunter;
    public Perk mercenary;
    public Perk noble;
    public Perk priest;
    public Perk scholar;
    public Perk soldier;
    public Perk survivalist;
    public Perk thespian;

    [Space]
    public Perk ammunitionRecovery;
    public Perk beastOfBurden;
    public Perk bloodMagic;
    public Perk cunningLinguist;
    public Perk debilitatingPoisons;
    public Perk drainTheWell;
    public Perk fastHealer_v1;
    public Perk fastHealer_v2;
    public Perk keenEyes;
    public Perk lastStand;
    public Perk secondWind;
    public Perk slowMetabolism;
    public Perk sturdyBuild;
    public Perk turnTheTides;

    public void ApplyPerkBonuses(Perk perk)
    {
        if (perk == cunningLinguist)
        {
            //total number of languages minus 2 (common and beast)
            for (int i = 0; i < System.Enum.GetNames(typeof(SpokenLanguage)).Length - 2; i++)
            {
                //IDK what to do from here
            }
            Debug.Log("Learn new Language");
            UIManager.instance.selectionPanel.DisplayOptions("lang1", "lang2", "lang3", "lang4", "lang5");
        }
        else if (perk == sturdyBuild)
        {
            PlayerStats.instance.statSheet.maxHealth.RaiseStat(50);
        }
        else if (perk == beastOfBurden)
        {
            var effect = new MinorStat_Reinforce();
            effect.spellSchool = SpellSchool.Transmutation;
            effect.minorAttribute = MinorAttribute.Carry_Capacity;
            PlayerStats.instance.AddSpellEffect(effect, PlayerStats.instance.statSheet.GetMinorAttributeIndex(MinorAttribute.Carry_Capacity), 50, 0, "Beast of Burden");
            //PlayerStats.instance.statSheet.carryCapacity.RaiseStat(50);
        }
    }
}