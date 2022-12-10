using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSpell : MonoBehaviour
{
    //A base class for all spell types
    [HideInInspector] public CharacterSpellcasting caster;
    public EffectHolder[] effects;
    public string spellName;
    public Spell spell;
    [Space]
    [Tooltip("Played while the spell is active")]
    public AudioClip activeAFX;
    [Tooltip("Played when the spell makes impact")]
    public AudioClip impactAFX;
}
