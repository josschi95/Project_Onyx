using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class JournalManager : MonoBehaviour
{
    #region - Singleton -
    private static JournalManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    QuestManager questManager;
    public List<NPC> knownCharacters = new List<NPC>();

    //public GameObject questPage, characterPage, bestiaryPage, craftingPage;
    public TMP_Text titleText;
    public Button questPageButton, charactersPageButton, bestiaryPageButton, craftingPageButton, logPageButton, logClearButton;
    public Transform infoPanelParent;
    private int activePage = 0;
    [Space]
    public TMP_Text headerRight;
    public TMP_Text details, entries;
    Quest displayedQuest;
    private List<string> notificationLogs = new List<string>();

    private void Start()
    {
        questManager = QuestManager.instance;

        questPageButton.onClick.AddListener(OnSelectQuests);
        charactersPageButton.onClick.AddListener(OnSelectCharacters);
        bestiaryPageButton.onClick.AddListener(OnSelectBestiary);
        //craftingPageButton.onClick.AddListener(OnSelectCrafting);
        logPageButton.onClick.AddListener(OnSelectLog);
        logClearButton.onClick.AddListener(ClearLog);
    }

    public void UpdateMenu()
    {
        if (activePage == 1) OnSelectCharacters();
        else if (activePage == 2) OnSelectBestiary();
        //else if (activePage == 3) OnSelectCrafting();
        else if (activePage == 4) OnSelectLog();
        else OnSelectQuests();
    }

    private void ClearPanelItems()
    {
        foreach (Transform child in infoPanelParent.transform.Cast<Transform>().ToList())
        {
            ObjectPooler.ReturnToPool_Static("infoPanelElement", child.gameObject);
        }
    }

    #region - Quest Page -
    private void OnSelectQuests()
    {
        activePage = 0;
        titleText.text = "Quests";
        DisplayActiveQuestList();
        DisplayQuestDetails(questManager.currentTrackedQuest);
    }

    private void DisplayActiveQuestList()
    {
        ClearPanelItems();

        foreach (Quest activeQuest in questManager.activeQuests)
        {
            GameObject newItem = ObjectPooler.SpawnFromPool_Static("infoPanelElement", transform.position, Quaternion.identity);
            InfoPanelChild txtItem = newItem.GetComponent<InfoPanelChild>();
            txtItem.button.onClick.RemoveAllListeners();

            txtItem.infoPanelTitle.text = activeQuest.questName;
            newItem.transform.SetParent(infoPanelParent);

            //Clicking on a quest displays the quest details on the right panel and sets it as the displayedQuest
            txtItem.button.onClick.AddListener(delegate
            {
                DisplayQuestDetails(activeQuest);
            });

            //Clicking on a quest when it is already displayed will toggle it on/off
            //Did I implement this or not?

            //Display an icon next to the currently active quest
            if (questManager.currentTrackedQuest == activeQuest)
                txtItem.activeQuestIcon.enabled = true;
            else txtItem.activeQuestIcon.enabled = false;

            //Clicking on the displayedQuest again will set track it or change it to null
            if (displayedQuest == activeQuest)
            {
                if (questManager.currentTrackedQuest == activeQuest)
                {
                    txtItem.button.onClick.AddListener(delegate
                    {
                        questManager.OnTrackedQuestChange(null);
                        DisplayActiveQuestList();
                    });
                }
                else
                {
                    txtItem.button.onClick.AddListener(delegate
                    {
                        questManager.OnTrackedQuestChange(activeQuest);
                        DisplayActiveQuestList();
                    });
                }
            }
        }
    }

    //Show completed, failed, and abandoned
    private void DisplayCompletedQuestList()
    {
        ClearPanelItems();

        foreach (Quest completedQuest in questManager.completedQuests)
        {
            GameObject newItem = ObjectPooler.SpawnFromPool_Static("infoPanelElement", transform.position, Quaternion.identity);
            InfoPanelChild txtItem = newItem.GetComponent<InfoPanelChild>();
            txtItem.button.onClick.RemoveAllListeners();

            txtItem.infoPanelTitle.text = completedQuest.questName;
            txtItem.activeQuestIcon.enabled = false;

            txtItem.button.onClick.AddListener(delegate
            {
                DisplayQuestDetails(completedQuest);
            });
            newItem.transform.SetParent(infoPanelParent);
        }
    }

    private void DisplayQuestDetails(Quest quest)
    {
        if (quest != null)
        {
            displayedQuest = quest;
            headerRight.text = quest.questName;
            details.text = quest.primaryQuestDescription;
        }
    }
    #endregion

    #region - Character Page -
    private void OnSelectCharacters()
    {
        activePage = 1;
        titleText.text = "Characters";
        DisplayKnownCharacters();
    }

    private void DisplayKnownCharacters()
    {
        ClearPanelItems();

        foreach (NPC character in knownCharacters)
        {
            GameObject newItem = ObjectPooler.SpawnFromPool_Static("infoPanelElement", transform.position, Quaternion.identity);
            InfoPanelChild txtItem = newItem.GetComponent<InfoPanelChild>();
            txtItem.button.onClick.RemoveAllListeners();

            txtItem.infoPanelTitle.text = character.name;
            txtItem.activeQuestIcon.enabled = false;

            txtItem.button.onClick.AddListener(delegate
            {
                DisplayCharacterDetails(character);
            });
            newItem.transform.SetParent(infoPanelParent);
        }
    }

    private void DisplayCharacterDetails(NPC character)
    {
        //Summary of learned information about character
        //affinity
        //known location
        //known associates
        //known faction (if any)
        //quests completed for them, noteworthy interations
        Debug.Log("Soon to be implemented");
    }

    public static void OnCharacterInteraction_Static(NPC character)
    {
        instance.OnCharacterInteraction(character);
    }

    private void OnCharacterInteraction(NPC character)
    {
        if (!knownCharacters.Contains(character))
        {
            knownCharacters.Add(character);
            UIManager.instance.AddNotification("Journal Updated: " + character.name);
        }
    }
    #endregion

    #region - Bestiary Page - 
    private void OnSelectBestiary()
    {
        activePage = 2;
        titleText.text = "Bestiary";
        ClearPanelItems();
        //Show list of encountered enemy types
        //Some kind of rundown of their... whatever
        //habits, environments, tactics, personalities
        //basically just lore stuff for people who are interested and would read the monster descriptions in the MM
    }
    #endregion

    #region - CraftingPage - 
    /*private void OnSelectCrafting()
    {
        activePage = 3;
        titleText.text = "Crafting";
        ClearPanelItems();
        //I think just a list of crafting recipes
        //known spell effects
        //probably a pretty minor inclusion
    }*/
    #endregion

    #region - Log Page -
    private void OnSelectLog()
    {
        activePage = 4;
        titleText.text = "Log";
        DisplayLog();
    }

    private void DisplayLog()
    {
        ClearPanelItems();

        foreach (string log in notificationLogs)
        {
            GameObject newItem = ObjectPooler.SpawnFromPool_Static("infoPanelElement", transform.position, Quaternion.identity);
            InfoPanelChild txtItem = newItem.GetComponent<InfoPanelChild>();
            txtItem.button.onClick.RemoveAllListeners();

            txtItem.infoPanelTitle.text = log;
            txtItem.activeQuestIcon.enabled = false;

            newItem.transform.SetParent(infoPanelParent);
        }
    }

    public static void AddLog_Static(string log)
    {
        instance.AddLog(log);
    }

    private void AddLog(string log)
    {
        notificationLogs.Add(log);
    }

    public void ClearLog()
    {
        notificationLogs.Clear();
        DisplayLog();
    }
    #endregion
}
