using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellEffect : ScriptableObject
{
    public string effectName;
    public SpellSchool spellSchool;
    public int baseCost = 1;
    public bool usesMagnitude = true;
    [Space]
    public AudioClip onCast;

    public virtual void ApplyEffect(Collider other, string name, float magnitude, float duration)
    {
        //Meant to be overwritten
    }

    public virtual void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        //Meant to be overwritten
    }
}
