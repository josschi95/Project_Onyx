using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Resist", menuName = "Spell Effects/Attribute Modifier/Resist Damage Effect")]
public class Resist_Damage : StatEffect
{
    public TypeResistance resistance;

    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.AddSpellEffect(this, character.statSheet.GetResistanceIndex(resistance), -newMag, duration, name);
    }

    public override void RemoveEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.RemoveSpellEffect(this, character.statSheet.GetResistance(resistance), -newMag, name);
    }
}
