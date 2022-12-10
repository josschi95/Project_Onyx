using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/ Quest")]
public class Quest : ScriptableObject
{
    #region - Callbacks - 
    //Likely use this as a trigger for certain world-events to take place for certain quests
    public delegate void OnQuestStart(Quest quest);
    public OnQuestStart onQuestStartCallback;

    //A callback for when the quest state advances
    //Currently used to update the compass bar quest marker
    public delegate void OnQuestAdvanced(Quest quest);
    public OnQuestAdvanced onQuestAdvanced;

    //Currently no uses
    public delegate void OnQuestEnd(Quest quest);
    public OnQuestEnd onQuestEndCallback;
    #endregion

    public int questID;
    [Tooltip("Needs to match quest name in Dialogue Manager")]
    public string questName;
    public PixelCrushers.DialogueSystem.QuestState questState = PixelCrushers.DialogueSystem.QuestState.Unassigned;
    [Space]
    public QuestState[] questStates;
    public QuestState currentState;
    public int currentStateNum { get; private set; }
    [Space]
    [TextArea(3, 5)] public string primaryQuestDescription;

    [Header("Quest Chaining")]
    public Quest prerequisiteQuest;
    public Quest disqualifyingQuest;
    public Quest questOnCompletion;

    [Header("Quest Rewards")]
    public int questReward_Coins;
    public CurrencyType coinType;
    public InventoryItem[] questReward_Items;
    public Recipe[] questRewardRecipes;

    [Header("Quest Requisite Items")]
    public InventoryItem[] requisiteItems;
    public Recipe[] requisiteRecipes;

    //Have the requirements for this quest been met
    public bool QuestRequirementsMet()
    {
        //I haven't gotten around to implementing it, but I just remembered the QuestState.Grantable. I can likely use this to avoid all of this nonsense
        //This quest is not tied to any other quests
        if (prerequisiteQuest == null && disqualifyingQuest == null) return true;

        if (disqualifyingQuest != null)
        {
            //I suppose this would be more like... not requirements but accessible
            if (disqualifyingQuest.questState == PixelCrushers.DialogueSystem.QuestState.Active) return false;
            if (disqualifyingQuest.questState == PixelCrushers.DialogueSystem.QuestState.Success) return false;
        }
        else if (prerequisiteQuest != null && prerequisiteQuest.questState == PixelCrushers.DialogueSystem.QuestState.Success) return true;
        return false;
    }

    #region - Quest State Change -
    //Add quest to list of currently active quests
    public virtual void ActivateQuest()
    {
        questState = PixelCrushers.DialogueSystem.QuestState.Active;
        QuestLog.SetQuestState(questName, questState);
        QuestManager.instance.OnNewQuestAdded(this);

        CheckForQuestProgression();

        if (onQuestStartCallback != null)
            onQuestStartCallback.Invoke(this);
    }

    //Advances quest to given state
    public void AdvanceQuest(QuestState newState)
    {
        if (currentState == newState) return;

        Debug.Log("Advancing " + questName + " to " + newState.name);

        currentState.OnStateExit();
        currentState = newState;
        newState.OnStateEnter(this);

        //Set current state num for simple saving reference
        for (int i = 0; i < questStates.Length; i++)
        {
            if (currentState == questStates[i])
            {
                currentStateNum = i;
                break;
            }
        }

        questState = currentState.parentState;
        QuestLog.SetQuestState(questName, questState);
        Debug.Log(questName + ": to, " + QuestLog.GetQuestState(questName));

        if (questState == PixelCrushers.DialogueSystem.QuestState.Success) CompleteQuest();
        else if (questState == PixelCrushers.DialogueSystem.QuestState.Failure) FailQuest();

        if (onQuestAdvanced != null) onQuestAdvanced.Invoke(this);
    }

    //The player manually abandoned the quest
    public virtual void AbandonQuest()
    {
        questState = PixelCrushers.DialogueSystem.QuestState.Abandoned;
        if (onQuestEndCallback != null) onQuestEndCallback.Invoke(this);

    }

    //The quest entered a fail state
    public virtual void FailQuest()
    {
        questState = PixelCrushers.DialogueSystem.QuestState.Failure;
        if (onQuestEndCallback != null) onQuestEndCallback.Invoke(this);
    }

    public virtual void CompleteQuest()
    {
        questState = PixelCrushers.DialogueSystem.QuestState.Success;
        if (onQuestEndCallback != null) onQuestEndCallback.Invoke(this);

        //Remove questItems, not sure if this should be in here or in the objective, possibly need a bool for both
        foreach (QuestState state in questStates)
        {
            if (state.loseItemsOnQuestEnd == true) state.RemoveQuestItems();
        }

        GrantQuestRewards();

        //This will likely be the new way I implement this, accompanied by some other logic such as quests this grants access to and quests that it disqualifies
        if (questOnCompletion != null && questOnCompletion.questState == PixelCrushers.DialogueSystem.QuestState.Grantable)
        {
            //questOnCompletion.ActivateQuest();
        }
        if (questOnCompletion != null && questOnCompletion.QuestRequirementsMet())
        {
            //if the requirements aren't met (e.g. there's more than one requirement?
            //then it won't trigger unless the other requirements also can grant it
            questOnCompletion.ActivateQuest();
        }
    }
    #endregion

    //Advance to highest state with all conditions met
    public virtual void CheckForQuestProgression()
    {
        int num = currentState.stateHierarchy;

        //This is likely the best place to see if I need to revert to a previous state for those that can fluxuate (collect quests)

        for (int i = 0; i < questStates.Length; i++)
        {
            if (questStates[i] != currentState) //Don't bother checking current state
            {
                if (questStates[i].stateHierarchy >= num && questStates[i].StateRequirementsMet())
                {
                    num = i;
                    AdvanceQuest(questStates[num]);
                    //Ideally I would have all states in hierarchical order so the player would advance through all of the underlying states
                }
            }
        }
    }

    //Currenlty just calling this from DialogueManager as a workaround
    public virtual void GiveRequisiteItems()
    {
        for (int i = 0; i < requisiteItems.Length; i++)
        {
            //PlayerInventory.instance.AddItem(requisiteItems[i].item, requisiteItems[i].quantity);
            PlayerInventory.instance.AddItem(requisiteItems[i].itemID, requisiteItems[i].quantity);
        }
        for (int i = 0; i < requisiteRecipes.Length; i++)
        {
            PlayerInventory.instance.LearnRecipe(requisiteRecipes[i]);
        }
    }

    //Grant quest rewards on completion
    public virtual void GrantQuestRewards()
    {
        if (questReward_Coins != 0)
        {
            PlayerInventory.instance.AddCurrency(coinType, questReward_Coins);
        }

        for (int i = 0; i < questReward_Items.Length; i++)
        {
            //PlayerInventory.instance.AddItem(questReward_Items[i].item, questReward_Items[i].quantity);
            PlayerInventory.instance.AddItem(questReward_Items[i].itemID, questReward_Items[i].quantity);
        }

        for (int i = 0; i < questRewardRecipes.Length; i++)
        {
            PlayerInventory.instance.LearnRecipe(questRewardRecipes[i]);
        }
    }

    //Helper function called from Editor
    public void ResetQuest()
    {
        questState = PixelCrushers.DialogueSystem.QuestState.Unassigned;
        currentState = questStates[0];
        for (int i = 0; i < questStates.Length; i++)
        {
            questStates[i].ResetQuestState();
        }
    }
}

//public enum QuestState { unassigned, active, success, failure, done, abandoned, granted, returnToNPC }