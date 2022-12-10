using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject questPosters;
    public List<Quest> questList = new List<Quest>();

    private void Start()
    {
        UpdateQuests();
    }

    public float interactionDistance = 2;

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        return true;
    }

    public void Interact(CharacterController controller)
    {
        if (controller is PlayerController)
        {
            if (questList.Count > 0)
            {
                UIManager.instance.DisplayQuestBoardScreen(this);
            }
            else
            {
                UIManager.instance.AddNotification("No available jobs");
            }
        }
    }

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString("Read", "Jobs Board", false);
    }

    public void UpdateQuests()
    {
        if (questList.Count > 0) questPosters.SetActive(true);
        else questPosters.SetActive(false);
    }
}
