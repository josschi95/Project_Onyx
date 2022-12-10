using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Catapult", menuName = "Spell Effects/Other/Catapult")]

public class Effect_Catapult : StatEffect
{
    public override void ApplyEffect(CharacterStats character, string name, float magnitude, float duration)
    {
        Debug.Log(character.transform.name);
        character.rb.AddForce(character.transform.up * magnitude);
        character.rb.AddForce(character.transform.forward * magnitude);
    }
}
