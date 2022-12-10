using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At the moment, I'm only using this for defeating targets
/// So just get that working and worry about the rest later
/// </summary>

public class QuestTrigger : MonoBehaviour
{
    public Objective objectiveType = Objective.Defeat;
    public QuestState relevantState;
    public int conditionNum;

    private void Start()
    {
        switch (objectiveType)
        {
            case Objective.Defeat:
                {
                    var character = GetComponentInParent<CharacterStats>();
                    character.onDeath += OnTargetDeath;
                    character.onUnconscious += OnTargetUnconscious;
                    break;
                }
            case Objective.GoTo:
                {
                    Debug.Log("Yet to be implemented");
                    break;
                }
        }
    }

    private void OnTargetDeath()
    {
        relevantState.stateConditions[conditionNum].currentNum++;
        relevantState.parentQuest.CheckForQuestProgression();
    }

    private void OnTargetUnconscious()
    {
        if (relevantState.stateConditions[conditionNum].canKnockUnconscious)
        {
            relevantState.stateConditions[conditionNum].currentNum++;
        }
        relevantState.parentQuest.CheckForQuestProgression();
    }
}