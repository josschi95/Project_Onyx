using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class QuestManager : MonoBehaviour
{
    #region - Singleton -
    public static QuestManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of QuestManager found");
            return;
        }

        instance = this;
    }
    #endregion

    public delegate void OnTrackedQuestChanged(Quest quest);
    public OnTrackedQuestChanged onTrackedQuestChanged;

    public Quest currentTrackedQuest { get; private set; }
    [Space]
    public List<Quest> activeQuests = new List<Quest>();

    public List<Quest> completedQuests = new List<Quest>();
    public List<Quest> failedQuests = new List<Quest>();
    public List<Quest> abandonedQuests = new List<Quest>();

    public void Start()
    {
        PlayerInventory.instance.onItemChangedCallback += UpdateCollectionQuests;
    }

    //Adds a new quest to the list of active quests that will appear in the player's Quest Journal
    public void OnNewQuestAdded(Quest quest)
    {
        UIManager.instance.AddNotification("New Quest: " + quest.questName);
        quest.onQuestEndCallback += OnQuestEnded;
        activeQuests.Add(quest);

        if (currentTrackedQuest == null)
            OnTrackedQuestChange(quest);
    }

    //Sets the currently tracked quest to the given one
    public void OnTrackedQuestChange(Quest newQuest)
    {
        currentTrackedQuest = newQuest;
        if (onTrackedQuestChanged != null) onTrackedQuestChanged.Invoke(newQuest);
    }

    //function called whenever there is a change in the player's inventory to check for any collect quest
    public void UpdateCollectionQuests()
    {
        foreach (Quest activeQuest in activeQuests)
        {
            for (int i = 0; i < activeQuest.questStates.Length; i++)
            {
                if (activeQuest.questStates[i].conditionsObjective == Objective.Collect)
                {
                    activeQuest.CheckForQuestProgression();
                }
            }
        }
    }

    //Returns all currently active quests
    public void AllActiveQuests()
    {
        string[] activeQuests = QuestLog.GetAllQuests();
    }

    //Remove a quest from the list of active quests and add to list of completed, failed, or abandoned quests
    public void OnQuestEnded(Quest quest)
    {
        quest.onQuestEndCallback -= OnQuestEnded;
        activeQuests.Remove(quest);
        QuestLog.SetQuestState(quest.questName, quest.questState);
        if (currentTrackedQuest == quest) OnTrackedQuestChange(null);

        switch (quest.questState)
        {
            case PixelCrushers.DialogueSystem.QuestState.Success:
                {
                    UIManager.instance.AddNotification(quest.questName + " completed");
                    completedQuests.Add(quest);
                    break;
                }
            case PixelCrushers.DialogueSystem.QuestState.Failure:
                {
                    UIManager.instance.AddNotification(quest.questName + " failed");
                    failedQuests.Add(quest);
                    break;
                }
            case PixelCrushers.DialogueSystem.QuestState.Abandoned:
                {
                    UIManager.instance.AddNotification(quest.questName + " abandoned");
                    abandonedQuests.Add(quest);
                    break;
                }
        }
    }

    public void SetSavedValues(Quest current, List<Quest> active, List<int> activeStage, List<Quest> completed, List<Quest> failed, List<Quest> abandoned)
    {
        for (int i = 0; i < active.Count; i++)
        {
            active[i].ResetQuest();
            active[i].ActivateQuest();
            active[i].AdvanceQuest(active[i].questStates[activeStage[i]]);
        }
        if (current != null) OnTrackedQuestChange(current);

        for (int i = 0; i < completed.Count; i++)
        {
            completed[i].questState = PixelCrushers.DialogueSystem.QuestState.Success;
            QuestLog.SetQuestState(completed[i].questName, completed[i].questState);
            completedQuests.Add(completed[i]);
        }
        for (int i = 0; i < failed.Count; i++)
        {
            failed[i].questState = PixelCrushers.DialogueSystem.QuestState.Failure;
            QuestLog.SetQuestState(failed[i].questName, failed[i].questState);
            failedQuests.Add(failed[i]);
        }
        for (int i = 0; i < abandoned.Count; i++)
        {
            abandoned[i].questState = PixelCrushers.DialogueSystem.QuestState.Abandoned;
            QuestLog.SetQuestState(abandoned[i].questName, abandoned[i].questState);
            abandonedQuests.Add(abandoned[i]);
        }
    }
}

//public void OnObjectiveStart(QuestState state) { }
//public void OnObjectiveEnd(QuestState state) { }