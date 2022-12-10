using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Active Effect", menuName = "Spell Effects/Damage Effect")]
public class DamageEffect : StatEffect
{
    public DamageType type;

    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        if (duration <= 1) character.ApplyDamage(null, magnitude, type, true);
        else character.ApplyDamageOverTime(magnitude, type, true, duration);
    }
}
