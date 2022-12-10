using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invincibility", menuName = "Spell Effects/Other/Invincibility")]

public class Invincibility : StatEffect
{
    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        character.SetInvincible(duration);
    }
}
