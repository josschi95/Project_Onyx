using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Return", menuName = "Spell Effects/Other/Return")]
public class ReturnEffect : StatEffect
{
    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        //This is a permanenent change at the moment, so I definitely need to make this some sort of coroutine
        character.characterCombat.projectileReturn = true;
    }
}
