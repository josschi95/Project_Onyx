using UnityEngine;
using UnityEngine.UI;

public class SystemPanelManager : MonoBehaviour
{
    private UIManager UI;
    [SerializeField] private SettingsMenu settings;

    public bool isOpen { get; private set; }
    [SerializeField] private Button resumeButton, saveButton, loadButton, controlsButton, settingsButton, creditsButton, quitButton;
    [SerializeField] private GameObject mainPanel, buttonsParent, saveMenu, loadMenu, controlsMenu, settingsMenu, creditsMenu, quitMenu;

    private void Start()
    {
        UI = UIManager.instance;
        resumeButton.onClick.AddListener(OnClose);

        saveButton.onClick.AddListener(DisplaySaveMenu);

        loadButton.onClick.AddListener(DisplayLoadMenu);

        controlsButton.onClick.AddListener(DisplayControlsMenu);

        settingsButton.onClick.AddListener(DisplaySettingsMenu);

        creditsButton.onClick.AddListener(DisplayCreditsMenu);

        quitButton.onClick.AddListener(DisplayQuitMenu);
    }

    public void OnOpen()
    {
        isOpen = true;
        mainPanel.SetActive(true);
        buttonsParent.SetActive(true);

        saveMenu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        HUDManager.instance.hudLocked = true;
        GameMaster.instance.PauseGame();
        UIManager.instance.ToggleCursor(true);
    }

    public void OnClose()
    {
        isOpen = false;
        mainPanel.SetActive(false);

        saveMenu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        if (UI.playerMenuOpen)
        {
            //I don't know what I'll have to do here
        }
        else if (UI.dialogueMenuOpen || UI.containerMenuOpen || UI.inventoriesPanelOpen)
        {
            //Resume game but don't hide cursor
            GameMaster.instance.ResumeGame();
        }
        else
        {
            HUDManager.instance.hudLocked = false;
            GameMaster.instance.ResumeGame();
            UIManager.instance.ToggleCursor(false);
        }
    }

    public void DisplaySaveMenu()
    {
        GameMaster.instance.SaveGame();
        buttonsParent.SetActive(false);
        saveMenu.SetActive(true);
    }

    public void DisplayLoadMenu()
    {
        GameMaster.instance.LoadGame();
        buttonsParent.SetActive(false);
        loadMenu.SetActive(true);
    }

    public void DisplayControlsMenu()
    {
        buttonsParent.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void DisplaySettingsMenu()
    {
        buttonsParent.SetActive(false);
        settingsMenu.SetActive(true);
        settings.SetInitialValues();
    }

    public void DisplayCreditsMenu()
    {
        buttonsParent.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void DisplayQuitMenu()
    {
        buttonsParent.SetActive(false);
        quitMenu.SetActive(true);
    }

    public void ReturnToMain()
    {
        buttonsParent.SetActive(true);

        saveMenu.SetActive(false);
        loadMenu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        quitMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }
}
