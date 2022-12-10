using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Class", menuName = "Characters/Class")]
public class CharacterClass : ScriptableObject
{
    new public string name;
    public ClassArchetype archetype;
    [TextArea(1, 4)] public string classDescription;
    [Space]
    public MajorAttribute favoredAtt_01;
    public MajorAttribute favoredAtt_02;
    [Space]
    public SkillField primarySkill_00;
    public SkillField primarySkill_01;
    public SkillField primarySkill_02;
    public SkillField primarySkill_03;
    public SkillField primarySkill_04;
    [Space]
    public SkillField secondarySkill_00;
    public SkillField secondarySkill_01;
    public SkillField secondarySkill_02;
    public SkillField secondarySkill_03;
    public SkillField secondarySkill_04;

    public Item[] startingEquipment;
    public Spell[] startingSpells;

    public void ApplyClassPresets(CharacterStatSheet sheet)
    {
        Stat[] archetypeAtts = new Stat[5];
        switch (archetype)
        {
            case ClassArchetype.Warrior:
                {
                    archetypeAtts[0] = sheet.strength;
                    archetypeAtts[1] = sheet.endurance;
                    archetypeAtts[2] = sheet.finesse;
                    archetypeAtts[3] = sheet.social;
                    archetypeAtts[4] = sheet.intellect;
                    break;
                }
            case ClassArchetype.Mage:
                {
                    archetypeAtts[0] = sheet.intellect;
                    archetypeAtts[1] = sheet.social;
                    archetypeAtts[2] = sheet.finesse;
                    archetypeAtts[3] = sheet.endurance;
                    archetypeAtts[4] = sheet.strength;
                    break;
                }
            case ClassArchetype.Thief:
                {
                    archetypeAtts[0] = sheet.finesse;
                    archetypeAtts[1] = sheet.social;
                    archetypeAtts[2] = sheet.endurance;
                    archetypeAtts[3] = sheet.intellect;
                    archetypeAtts[4] = sheet.strength;
                    break;
                }
        }

        //Sets starting Attribute Array 
        archetypeAtts[0].SetBaseValue(40);  
        archetypeAtts[1].SetBaseValue(40);
        archetypeAtts[2].SetBaseValue(35);
        archetypeAtts[3].SetBaseValue(30);
        archetypeAtts[4].SetBaseValue(30);

        sheet.GetMajorAttribute(favoredAtt_01).RaiseStat(10);
        sheet.GetMajorAttribute(favoredAtt_02).RaiseStat(10);
        //total 195 starting Attribute points, with max of 500

        //Set all Skills to 5, Resistances to 0,
        //max health/mana/stamina to 100
        //healthRegen to 0, critMult to 3, armor to 0, carry cap to 3x STR
        sheet.ResetScores();

        Stat[] primarySkills = new Stat[5];
        primarySkills[0] = sheet.GetSkill(primarySkill_00);
        primarySkills[1] = sheet.GetSkill(primarySkill_01);
        primarySkills[2] = sheet.GetSkill(primarySkill_02);
        primarySkills[3] = sheet.GetSkill(primarySkill_03);
        primarySkills[4] = sheet.GetSkill(primarySkill_04);

        for (int i = 0; i < primarySkills.Length; i++)
            primarySkills[i].RaiseStat(20);

        Stat[] secondarySkills = new Stat[5];
        secondarySkills[0] = sheet.GetSkill(secondarySkill_00);
        secondarySkills[1] = sheet.GetSkill(secondarySkill_01);
        secondarySkills[2] = sheet.GetSkill(secondarySkill_02);
        secondarySkills[3] = sheet.GetSkill(secondarySkill_03);
        secondarySkills[4] = sheet.GetSkill(secondarySkill_04);

        for (int i = 0; i < secondarySkills.Length; i++)
            secondarySkills[i].RaiseStat(10);
    }
}
public enum ClassArchetype { Warrior, Mage, Thief }
