using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMenuManager : MonoBehaviour
{
    [SerializeField] Animator anim;
    [HideInInspector] public bool isOpen { get; private set; }
    [HideInInspector] public bool characterOpen, inventoryOpen, journalOpen, mapOpen;
    public GameObject characterPanel, inventoryPanel, journalPanel, mapPanel;
    public Button characterButton, inventoryButton, journalButton, mapButton;

    [SerializeField] private CharacterPanelManager characterPanelManager;
    [SerializeField] private PlayerInventoryDisplay inventoryPanelManager;
    [SerializeField] private JournalManager journalManager;
    //[SerializeField] MapManager mapManager;

    private void Start()
    {
        isOpen = false;
        characterButton.onClick.AddListener(delegate
        {
            characterPanel.SetActive(true);
            inventoryPanel.SetActive(false);
            journalPanel.SetActive(false);
            mapPanel.SetActive(false);

            characterPanelManager.UpdateMenu();
        });

        inventoryButton.onClick.AddListener(delegate
        {
            characterPanel.SetActive(false);
            inventoryPanel.SetActive(true);
            journalPanel.SetActive(false);
            mapPanel.SetActive(false);

            inventoryPanelManager.UpdatePanel();
        });

        journalButton.onClick.AddListener(delegate
        {
            characterPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            journalPanel.SetActive(true);
            mapPanel.SetActive(false);

            journalManager.UpdateMenu();
        });

        mapButton.onClick.AddListener(delegate
        {
            characterPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            journalPanel.SetActive(false);
            mapPanel.SetActive(true);

        });
    }

    #region - Menu Toggling -
    public void Open()
    {
        isOpen = true;
        anim.Play("menuOpen");
        UIManager.instance.OnMenuOpen();
        inventoryPanelManager.OnMenuOpen();
        if (characterPanel.activeSelf == true) characterPanelManager.UpdateMenu();
        else if (inventoryPanel.activeSelf == true) inventoryPanelManager.UpdatePanel();
        else journalManager.UpdateMenu();
        //else (mapPanel.activeSelf == true) mapManager.UpdateMenu();
    }

    public void Close()
    {
        isOpen = false;
        anim.Play("menuClose");
        UIManager.instance.OnMenuClose();
        inventoryPanelManager.OnMenuClose();
    }

    public void OpenCharacterMenu()
    {
        CloseAllMenuScreens();
        characterPanel.SetActive(true);
        characterPanelManager.UpdateMenu();
    }

    public void OpenInventoryMenu()
    {
        CloseAllMenuScreens();
        inventoryPanel.SetActive(true);
        inventoryPanelManager.UpdatePanel();
    }

    public void OpenJournalMenu()
    {
        CloseAllMenuScreens();
        journalPanel.SetActive(true);
        journalManager.UpdateMenu();
    }

    public void OpenMapMenu()
    {
        CloseAllMenuScreens();
        mapPanel.SetActive(true);
        //mapManager.UpdateMenu();
    }

    public void CloseAllMenuScreens()
    {
        characterPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        journalPanel.SetActive(false);
        mapPanel.SetActive(false);
    }
    #endregion
}
