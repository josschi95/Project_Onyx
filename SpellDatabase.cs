using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell Database", menuName = "Databases/Spell Database")]
public class SpellDatabase : ScriptableObject
{
    public Spell[] spells;
    public Dictionary<int, Spell> GetSpell = new Dictionary<int, Spell>();

    public void AssignIDs()
    {
        GetSpell = new Dictionary<int, Spell>();
        for (int i = 0; i < spells.Length; i++)
        {
            GetSpell.Add(i, spells[i]);
            spells[i].spellID = i;
        }
    }
}
