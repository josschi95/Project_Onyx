using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability Database", menuName = "Databases/Ability Database")]
public class AbilityDatabase : ScriptableObject
{
    public Ability[] abilities;
    public Dictionary<int, Ability> GetAbility = new Dictionary<int, Ability>();

    public void AssignIDs()
    {
        GetAbility = new Dictionary<int, Ability>();
        for (int i = 0; i < abilities.Length; i++)
        {
            GetAbility.Add(i, abilities[i]);
            abilities[i].abilityID = i;
        }
    }
}
