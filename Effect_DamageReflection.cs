using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Reflection", menuName = "Spell Effects/Other/Damage Reflection")]
public class Effect_DamageReflection : StatEffect
{
    public DamageType type;
    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        var reflect = character.gameObject.AddComponent<DamageReflection>();
        reflect.duration = duration;
        reflect.reflectedPercent = magnitude * 0.01f;
        reflect.reflectedDamageType = type;
    }
}
