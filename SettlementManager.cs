using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementManager : MonoBehaviour
{
    public string cityName;
    public QuestBoard questBoard;
    public List<NPC> citizens = new List<NPC>();
    public List<Quest> localQuests = new List<Quest>();
    [Tooltip("Random chitchat, barks, or dialogue options to lead the player to a quest or point of interest")]
    public string[] localRumors;
    [Tooltip("List of titles acquired by the player, for reference by dialogueSystem")]
    public string[] knownPlayertitles;

    // Start is called before the first frame update
    void Start()
    {
        questBoard.gameObject.name = cityName + " Quest Board";
        foreach (NPC character in citizens)
        {
            //character.onNPCDeathCallback += TransferQuests;
        }
    }

    //Right now this will only move over unassigned quests, will need to find a way to solve 
    public void TransferQuests(NPC character)
    {
        foreach (Quest quest in character.questsToGive)
        {
            if (quest.questState == PixelCrushers.DialogueSystem.QuestState.Unassigned)
            {
                questBoard.questList.Add(quest);
                questBoard.UpdateQuests();
            }
        }
    }
}
