using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reinforce Skill ", menuName = "Spell Effects/Attribute Modifier/Skill Reinforce Effect")]
public class Skill_Reinforce : StatEffect
{
    public SkillField skill;

    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.AddSpellEffect(this, character.statSheet.GetSkillIndex(skill), newMag, duration, name);
    }

    public override void RemoveEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.RemoveSpellEffect(this, character.statSheet.GetSkill(skill), newMag, name);
    }
}
