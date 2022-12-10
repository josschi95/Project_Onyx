using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEffect : SpellEffect
{
    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        //base.ApplyEffect(character, name, magnitude, duration);
    }
}

/* Mainly Conjuration spells
 * monsters, equipment, animated weapons, etc.
 * 
 * But also evocation
 * elemental walls, spellNovas
 * 
 * Maybe transmutation
 * Wall of Stone, Throne of Dirt
 * 
 * Divination
 * Arcane/Divine Eye
 * 
 */