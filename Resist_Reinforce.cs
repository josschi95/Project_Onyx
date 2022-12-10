using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reinforce Resist", menuName = "Spell Effects/Attribute Modifier/Resist Reinforce Effect")]
public class Resist_Reinforce : StatEffect
{
    public TypeResistance resistance;

    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        //character.AddResistanceSpellEffect(this, resistance, newMag, duration, name);
        //character.AddSpellEffect(this, character.statSheet.GetResistance(resistance), newMag, duration, name);
        character.AddSpellEffect(this, character.statSheet.GetResistanceIndex(resistance), newMag, duration, name);
    }

    public override void RemoveEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.RemoveSpellEffect(this, character.statSheet.GetResistance(resistance), newMag, name);
    }
}
