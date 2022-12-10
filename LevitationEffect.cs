using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationEffect : StatEffect
{
    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        character.characterController.OnLevitateStart(duration);
    }
}
