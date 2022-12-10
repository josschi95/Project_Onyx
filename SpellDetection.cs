using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Detection Skill ", menuName = "Spell Effects/Detection Spells/Detect General")]
public class SpellDetection : SpellEffect
{
    [SerializeField] GameObject divinationDetection;
    public DivinedObjects revealedTargets;

    public override void ApplyEffect(CharacterStats character, string name, float radius, float duration)
    {
        if (character is PlayerStats player)
        {
            GameObject go = Instantiate(divinationDetection, player.transform);
            go.TryGetComponent(out ActiveDivination divination);
            divination.area = radius;
            divination.duration = duration;
            divination.revealedTargets = revealedTargets;
        }
    }
}
public enum DivinedObjects { Creatures, Life, Undead, Demons, Traps, Containers, Hidden, Curse, Disease, Poison, Plants }
