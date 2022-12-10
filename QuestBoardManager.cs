using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class QuestBoardManager : MonoBehaviour
{
    public Animator anim;
    public TMP_Text cityName;
    [SerializeField] Transform questPanelParent;
    [SerializeField] GameObject questPanelChild;
    public QuestBoard questboard;
    [Header("Quest Details")]
    public TMP_Text displayQuestName;
    public TMP_Text displayQuestDetails;
    public Button acceptQuestButton;
    [Space]
    [SerializeField] Button closeButton;
    [SerializeField] Vector3 closeButtonPos1;
    [SerializeField] Vector3 closeButtonPos2;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void ClearPanelItems()
    {
        foreach (Transform child in questPanelParent.transform.Cast<Transform>().ToList())
        {
            ObjectPooler.ReturnToPool_Static("infoPanelElement", child.gameObject);
        }
    }

    public void DisplayAvailableQuests(QuestBoard board)
    {
        ClearPanelItems();
        displayQuestName.text = "";
        displayQuestDetails.text = "";
        acceptQuestButton.enabled = false;
        closeButton.transform.localPosition = closeButtonPos1;

        foreach (Quest quest in board.questList)
        {
            GameObject newItem = ObjectPooler.SpawnFromPool_Static("infoPanelElement", transform.position, Quaternion.identity);
            InfoPanelChild txtItem = newItem.GetComponent<InfoPanelChild>();
            txtItem.button.onClick.RemoveAllListeners();

            ButtonSettings(txtItem, quest, board);
            txtItem.activeQuestIcon.enabled = false;
            txtItem.infoPanelTitle.text = quest.questName;

            newItem.transform.SetParent(questPanelParent);
        }

        closeButton.onClick.AddListener(delegate
        {
            CloseQuestBoardScreen();
        });
    }

    void ButtonSettings(InfoPanelChild txtItem, Quest quest, QuestBoard board)
    {
        txtItem.button.onClick.AddListener(delegate
        {
            DisplayQuestDetails(quest, board);
        });
    }

    void DisplayQuestDetails(Quest quest, QuestBoard board)
    {
        anim.SetBool("displayQuest", true);
        anim.Play("displayQuest");
        displayQuestName.text = quest.questName;
        displayQuestDetails.text = quest.primaryQuestDescription;
        acceptQuestButton.enabled = true;
        acceptQuestButton.onClick.AddListener(delegate
        {
            quest.ActivateQuest();
            board.questList.Remove(quest);
            DisplayAvailableQuests(board);
            anim.SetBool("displayQuest", false);
        });
        closeButton.transform.localPosition = closeButtonPos2;
    }

    public void CloseQuestBoardScreen()
    {
        ClearPanelItems();
        anim.SetTrigger("close");
        UIManager.instance.OnMenuClose();
    }
}
