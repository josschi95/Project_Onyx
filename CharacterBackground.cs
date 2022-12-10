using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Background", menuName = "Characters/Background")]
public class CharacterBackground : ScriptableObject
{
    new public string name;
    [TextArea(1, 4)] public string backgroundDescription;
    [Space]
    public SkillField skillBonus00;
    public SkillField skillBonus01;
    public SkillField skillBonus02;
    [Space]
    public Item[] backgroundItems;
    public Perk backgroundPerk;

    public void ApplyBackgroundPresets(CharacterStatSheet sheet)
    {
        Stat[] skills = new Stat[3];
        skills[0] = sheet.GetSkill(skillBonus00);
        skills[1] = sheet.GetSkill(skillBonus01);
        skills[2] = sheet.GetSkill(skillBonus02);

        for (int i = 0; i < skills.Length; i++)
            skills[i].RaiseStat(10);
    }
}
