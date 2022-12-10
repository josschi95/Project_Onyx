using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect : SpellEffect
{
    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        //Meant to be overwritten
    }

    public virtual void RemoveEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        //Meant to be overwritten
    }
}