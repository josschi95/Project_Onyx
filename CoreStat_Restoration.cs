using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Active Effect", menuName = "Spell Effects/Core Restoration Effect")]
public class CoreStat_Restoration : StatEffect
{
    public CoreAttribute coreAttribute;

    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        switch (coreAttribute)
        {
            case CoreAttribute.Health:
                {
                    character.RestoreHealth(magnitude);
                    break;
                }
            case CoreAttribute.Stamina:
                {
                    character.RestoreStamina(magnitude);
                    break;
                }
            case CoreAttribute.Mana:
                {
                    character.RestoreMana(magnitude);
                    break;
                }
        }
    }
}
