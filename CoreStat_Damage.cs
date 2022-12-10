using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Core Effect", menuName = "Spell Effects/Attribute Modifier/Damage Core Effect")]
public class CoreStat_Damage : StatEffect
{
    public CoreAttribute coreAttribute;

    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.AddSpellEffect(this, character.statSheet.GetCoreStatIndex(coreAttribute), -newMag, duration, name);
    }

    public override void RemoveEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        int newMag = Mathf.RoundToInt(magnitude);
        character.RemoveSpellEffect(this, character.statSheet.GetCoreAttribute(coreAttribute), -newMag, name);
    }
}
