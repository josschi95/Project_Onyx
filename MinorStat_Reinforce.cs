using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reinforce Minor Effect", menuName = "Spell Effects/Attribute Modifier/Minor Reinforce Effect")]
public class MinorStat_Reinforce : StatEffect
{
    public MinorAttribute minorAttribute;

    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.AddSpellEffect(this, character.statSheet.GetMinorAttributeIndex(minorAttribute), newMag, duration, name);
    }

    public override void RemoveEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.RemoveSpellEffect(this, character.statSheet.GetMinorAttribute(minorAttribute), newMag, name);
    }
}
