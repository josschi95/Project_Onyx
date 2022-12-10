using UnityEngine;

[System.Serializable]
public class EffectHolder
{
    public SpellEffect spellEffect;
    public SpellRange effectRange;
    public float magnitude = 1;
    public float duration = 0;
    public float area = 0;

    public string EffectDescription()
    {
        string details = spellEffect.effectName + " ";
        if (spellEffect.usesMagnitude) details += magnitude + " pts ";
        if (duration > 1) details += "for " + duration + "s ";
        if (area > 1) details += "in " + area + "m ";
        details += "on " + effectRange.ToString();
        return details;
    }

    public void ApplyGeneralEffect(Collider other, string name)
    {
        spellEffect.ApplyEffect(other, name, magnitude, duration);
    }

    //Call this when I for sure am getting a CharacterStats to target
    public void ApplyStatEffect(CharacterStats character, string name)
    {
        if (spellEffect is StatEffect statEffect) statEffect.ApplyEffect(character, name, magnitude, duration);
        else if (spellEffect is SpellDetection detection) detection.ApplyEffect(character, name, area, duration);
        else Debug.LogWarning("Something going on here");
    }

    #region - Equipment Only -
    public void RemoveStatEffect(CharacterStats character, string name)
    {
        if (spellEffect is StatEffect statEffect) statEffect.RemoveEffect(character, name, magnitude, duration);
        else Debug.LogWarning("Something going on here");
    }
    #endregion
}