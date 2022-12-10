using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reinforce Major Effect", menuName = "Spell Effects/Attribute Modifier/Major Reinforce Effect")]
public class MajorStat_Reinforce : StatEffect
{
    public MajorAttribute majorAttribute;

    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.AddSpellEffect(this, character.statSheet.GetAttributeIndex(majorAttribute), newMag, duration, name);
    }

    public override void RemoveEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.RemoveSpellEffect(this, character.statSheet.GetMajorAttribute(majorAttribute), newMag, name);
    }
}
