using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

[CreateAssetMenu(fileName = "New Quest State", menuName = "Quests/ Quest State")]
public class QuestState : ScriptableObject
{
    #region - Conditions -
    [System.Serializable]
    public class QuestStateCondition
    {
        public bool conditionMet;
        public bool dialogueOnly;
        public Objective conditionType;
        public int targetNum;
        public int currentNum;
        [Space]
        public QuestState prerequisiteQuest;

        public Item itemToCollect;

        public CreatureType creatureToDefeat;
        public bool canKnockUnconscious;

        public string locationToGo;

        public NPC relevantCharacter;
        //Probably change this to some sort of enum. 0 = alive, 1 = dead
        public bool characterAliveness;

        public bool ConditionMet()
        {
            if (dialogueOnly) return false;
            if (conditionMet || conditionType == Objective.None) return true;
            if (itemToCollect != null)
            {
                currentNum = PlayerInventory.instance.QueryItemCount(itemToCollect.itemID);
                if (currentNum >= targetNum)
                {
                    conditionMet = true;
                    return true;
                }
                return false;
            }
            else if (conditionType == Objective.Defeat)
            {
                //This will always reference specific targets, so make sure to have QuestTrigger components on them
                if (currentNum >= targetNum)
                {
                    conditionMet = true;
                    return true;
                }
                return false;
            }
            else if (conditionType == Objective.GoTo)
            {
                if (PlayerManager.instance.playerLocation == locationToGo) return true;
                return false;
            }
            else if (relevantCharacter != null)
            {
                if (characterAliveness == true)
                {
                    //Character must be alive
                    if (relevantCharacter.isAlive) return true;
                }
                else
                {
                    //Character must be dead
                    if (!relevantCharacter.isAlive)
                    {
                        conditionMet = true;
                        return true;
                    }
                }
            }
            return false;
        }
    }
    #endregion

    public Quest parentQuest;
    [Tooltip("Quest State cannot transition to a lower number")]
    public int stateHierarchy;
    [Space]
    public PixelCrushers.DialogueSystem.QuestState parentState = PixelCrushers.DialogueSystem.QuestState.Active;
    public string questMarkerName;
    [TextArea(3, 5)] public string journalEntry;

    [Tooltip("Summary of conditions required to enter this state")]
    public Objective conditionsObjective;
    public List<QuestStateCondition> stateConditions;

    public bool loseItemsOnObjectiveEnd = false;
    public bool loseItemsOnQuestEnd = false;

    //Query this from preceeding states to see if can progress to this state
    public virtual bool StateRequirementsMet()
    {
        //if any condition returns false, the requirements for this state have not been met
        for (int i = 0; i < stateConditions.Count; i++)
        {
            if (stateConditions[i].ConditionMet() == false)
            {
                if (parentQuest.currentState == this)
                {
                    Debug.LogWarning("Implement Fallback State");
                }
                return false;
            }
        }
        return true;
    }

    public virtual QuestState OnStateEnter(Quest quest)
    {
        //QuestManager.instance.OnObjectiveStart(this);
        
        //Update Journal
        //Likely don't need a return function here anymore
        return this;
    }   

    //Currently only being used to remove items from playerInventory
    public virtual void OnStateExit()
    {
        //QuestManager.instance.OnObjectiveEnd(this);
        if (loseItemsOnObjectiveEnd == true) RemoveQuestItems();
    }

    public void RemoveQuestItems()
    {
        for (int i = 0; i < stateConditions.Count; i++)
        {
            if (stateConditions[i].itemToCollect != null)
            {
                PlayerInventory.instance.RemoveItem(stateConditions[i].itemToCollect, stateConditions[i].targetNum);
            }
        }
    }

    //Helper function called from Editor
    public void ResetQuestState()
    {
        for (int i = 0; i < stateConditions.Count; i++)
        {
            stateConditions[i].currentNum = 0;
            stateConditions[i].conditionMet = false;
        }
    }
}
//None: Use for the first stage of every quest, conditions always met
public enum Objective { Collect, Defeat, GoTo, TalkTo, Wait, Other, None }

//functions used to set variables in DialogueManager and ensure it updates
//DialogueLua.SetVariable(dialogueSystemVariableName, currentNum);
//DialogueManager.instance.SendUpdateTracker();

//public delegate void OnObjectiveStart(QuestObjective objective);
//public OnObjectiveStart onObjectiveStart;

//public delegate void OnObjectiveComplete(QuestObjective objective);
//public OnObjectiveComplete onObjectiveComplete;