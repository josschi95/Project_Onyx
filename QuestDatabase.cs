using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Database", menuName = "Databases/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public Quest[] quests;
    public Dictionary<int, Quest> GetQuest = new Dictionary<int, Quest>();

    public void AssignIDs()
    {
        GetQuest = new Dictionary<int, Quest>();
        for (int i = 0; i < quests.Length; i++)
        {
            GetQuest.Add(i, quests[i]);
            quests[i].questID = i;
        }
    }
}
