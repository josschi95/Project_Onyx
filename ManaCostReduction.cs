using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mana Cost Reduction", menuName = "Spell Effects/Other/Mana Cost Reduction")]

public class ManaCostReduction : StatEffect
{
    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        if (character is PlayerStats player)
        {
            player.OnManaModifierChange(magnitude, duration);
        }
    }
}
