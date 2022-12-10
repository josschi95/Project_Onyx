using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC Database", menuName = "Databases/NPC Database")]
public class NPCDatabase : ScriptableObject
{
    public NPC[] NPCs;
    public Dictionary<int, NPC> GetNPC = new Dictionary<int, NPC>();

    public void AssignIDs()
    {
        GetNPC = new Dictionary<int, NPC>();
        for (int i = 0; i < NPCs.Length; i++)
        {
            GetNPC.Add(i, NPCs[i]);
            NPCs[i].npcID = i;
        }
    }
}
