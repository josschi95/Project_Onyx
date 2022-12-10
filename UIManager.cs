using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region - Singleton -
    public static UIManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of UI Manager found");
            return;
        }

        instance = this;
    }
    #endregion

    private PlayerController playerController;
    [SerializeField] private PlayerMenuManager playerMenu;
    public SystemPanelManager systemPanelManager;
    public HUDManager HUD;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPopUp;
    [SerializeField] private TMP_Text interactionMethod;
    [SerializeField] private TMP_Text interactionTarget;
    private Animator interactAnim;
    private float maxInteractionDistance = 10f;
    private IInteractable interactTarget;
    private bool checkForInteract = true;

    [Header("Popup Message")]
    [SerializeField] private GameObject reminderPopUp;
    [SerializeField] private TMP_Text reminderText;
    private Animator popupAnim;

    [Header("Notifications")]
    [SerializeField] private RectTransform displayBox;
    [SerializeField] private TMP_Text displayText;
    private Animator displayAnim;
    private Queue<string> displayMessages = new Queue<string>();
    private bool displayBoxActive;
    private float messageDelay = 2.5f; //May make this longer or shorter for certain messages

    [Header("Inventories Panel")]
    public GameObject inventoriesPanel;
    private Animator inventoriesAnim;
    public PlayerTransactionDisplay playerTransactionInventory;
    public InteractionInventoryDisplay interactionInventoryDisplay;

    //private MerchantInventory merchant;
    private NPC currentConversant;

    [Header("UI Elements")]
    public BossBarUI bossBar;
    public CraftingManager craftingManager;
    public QuestBoardManager questBoardScreen;
    public SelectionPanel selectionPanel;

    private Camera cam;

    #region - Menu Open Bools
    public bool playerMenuOpen { get; private set; }
    public bool containerMenuOpen { get; private set; }
    public bool inventoriesPanelOpen { get; private set; }
    public bool dialogueMenuOpen { get; private set; }
    #endregion

    private void Start()
    {
        HUD = HUDManager.instance;
        playerController = PlayerController.instance;
        cam = CameraHelper.instance.mainCam;
        interactAnim = interactionPopUp.GetComponent<Animator>();
        inventoriesAnim = inventoriesPanel.GetComponent<Animator>();
        popupAnim = reminderPopUp.GetComponent<Animator>();
        displayAnim = displayBox.GetComponent<Animator>();
        ToggleCursor(false);
        UpdateSpeechSkillScores();
    }

    private void Update()
    {
        if (checkForInteract == true)
        {
            var interactable = InteractableFound();
            if (interactable != null) DisplayInteract(interactable);
        }
    }

    public void OnMenuOpen()
    {
        playerMenuOpen = true;
        GameMaster.instance.PauseGame();
        ToggleCursor(true);
        checkForInteract = false;
        interactAnim.SetBool("interact", false);
    }

    public void OnMenuClose()
    {
        playerMenuOpen = false;
        GameMaster.instance.ResumeGame();
        ToggleCursor(false);
        checkForInteract = true;
        Tooltip_Item.HideTooltip_Static();
    }

    public void ToggleCursor(bool showCursor)
    {
        Cursor.visible = showCursor;
        if (showCursor) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    #region - Interaction -
    private IInteractable InteractableFound()
    {
        if (playerController.isInteracting == false)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.blue);
            if (Physics.Raycast(ray, out hit, maxInteractionDistance))
            {
                var interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    float dist = Vector3.Distance(hit.point, playerController.center.position);
                    if (interactable.DisplayPopup(dist) && interactable.CanBeAccessed(dist))
                        return interactable;
                }
            }
        }
        interactAnim.SetBool("interact", false);
        interactTarget = null;
        return null;
    }

    private void DisplayInteract(IInteractable interactable)
    {
        interactTarget = interactable;
        DoubleString display = interactable.GetInteractionDisplay();

        interactionMethod.text = display.interactionMethod;
        interactionTarget.text = display.interactionTarget;

        interactionMethod.color = Color.white;
        if (display.isCrime) interactionMethod.color = Color.red;

        interactAnim.SetBool("interact", true);
    }

    public void Interact()
    {
        if (GameMaster.instance.gamePaused || playerController.acceptInput == false) return;

        if (interactTarget == null) return;

        interactAnim.SetBool("interact", false);
        interactTarget.Interact(playerController);
        interactTarget = null;
    }
    #endregion

    #region - Player Menu -
    public void TogglePlayerMenu()
    {
        if (systemPanelManager.isOpen) return;

        if (playerMenu.isOpen) playerMenu.Close();

        else
        {
            if (containerMenuOpen == true)
            {
                CloseLootContainer();
                return;
            }
            else if (inventoriesPanelOpen == true)
            {
                CloseDoubleInventories();
                return;
            }
            else if (dialogueMenuOpen == true)
            {
                return;
            }

            //if (inventoriesPanelOpen == true) CloseMerchantScreen();
            //if (craftingScreenOpen == true) CloseCraftingScreen();

            playerMenu.Open();
        }
    }

    public void ToggleMenu_Character()
    {
        if (systemPanelManager.isOpen) return;

        if (playerMenu.isOpen)
        {
            if (playerMenu.characterPanel.activeSelf)
            {
                TogglePlayerMenu();
            }
            else playerMenu.OpenCharacterMenu();
        }
        else
        {
            TogglePlayerMenu();
            playerMenu.OpenCharacterMenu();
        }
    }

    public void ToggleMenu_Inventory()
    {
        if (systemPanelManager.isOpen) return;

        if (playerMenu.isOpen)
        {
            if (playerMenu.inventoryPanel.activeSelf)
            {
                TogglePlayerMenu();
            }
            else playerMenu.OpenInventoryMenu();
        }
        else
        {
            TogglePlayerMenu();
            playerMenu.OpenInventoryMenu();
        }
    }

    public void ToggleMenu_Journal()
    {
        if (systemPanelManager.isOpen) return;

        if (playerMenu.isOpen)
        {
            if (playerMenu.journalPanel.activeSelf)
            {
                TogglePlayerMenu();
            }
            else playerMenu.OpenJournalMenu();
        }
        else
        {
            TogglePlayerMenu();
            playerMenu.OpenJournalMenu();
        }
    }

    public void ToggleMenu_Map()
    {
        if (systemPanelManager.isOpen) return;

        if (playerMenu.isOpen)
        {
            if (playerMenu.mapPanel.activeSelf)
            {
                TogglePlayerMenu();
            }
            else playerMenu.OpenMapMenu();
        }
        else
        {
            TogglePlayerMenu();
            playerMenu.OpenMapMenu();
        }
    }

    public void ToggleMenu_System()
    {
        //Close system menu if it's open
        if (systemPanelManager.isOpen) systemPanelManager.OnClose();
        else
        {
            //Close player menu if it's open
            if (playerMenuOpen) TogglePlayerMenu();
            //Close loot container if searching one
            else if (containerMenuOpen) CloseLootContainer();
            else if (inventoriesPanelOpen) CloseDoubleInventories();
            else if (dialogueMenuOpen) return;
            //Else open system menu
            else systemPanelManager.OnOpen();
        }
    }
    #endregion

    #region - Inventory Menus -
    public void ContainerTakeAll()
    {
        if (containerMenuOpen == true && interactionInventoryDisplay.inventoryInteraction == InventoryInteraction.Looting)
        {
            interactionInventoryDisplay.TakeAll();
        }
        else if (inventoriesPanelOpen == true && interactionInventoryDisplay.inventoryInteraction == InventoryInteraction.Storage)
        {
            interactionInventoryDisplay.TakeAll();
        }
    }

    public void OpenLootContainer(Inventory inventory)
    {
        OnMenuOpen();
        containerMenuOpen = true;
        inventoriesAnim.Play("openSingle");
        interactionInventoryDisplay.OnStartTransaction(inventory, InventoryInteraction.Looting);
    }

    public void CloseLootContainer()
    {
        OnMenuClose();
        containerMenuOpen = false;
        inventoriesAnim.Play("closeSingle");
        interactionInventoryDisplay.OnEndTransaction();
    }

    public void OpenDoubleInventories(Inventory inventory, InventoryInteraction interactType)
    {
        OnMenuOpen();
        inventoriesPanelOpen = true;
        inventoriesAnim.Play("openDouble");
        interactAnim.SetBool("interact", false);
        interactionInventoryDisplay.OnStartTransaction(inventory, interactType);
        playerTransactionInventory.OnStartTransaction(inventory, interactType);
    }

    public void CloseDoubleInventories()
    {
        inventoriesPanelOpen = false;
        inventoriesAnim.Play("closeDouble");
        interactionInventoryDisplay.OnEndTransaction();
        playerTransactionInventory.OnEndTransaction();
        OnMenuClose();
    }

    public void CloseBarteringInventory()
    {
        GameMaster.instance.ResumeGame();
        inventoriesPanelOpen = false;
        inventoriesAnim.Play("closeDouble");
        interactionInventoryDisplay.OnEndTransaction();
        playerTransactionInventory.OnEndTransaction();
    }
    #endregion

    #region - Dialogue -
    public void OnDialogueStart(NPC character, string dialogueOverride, Transform actor)
    {
        //Set Variables first
        DialogueLua.SetVariable("playerWeaponsDrawn", PlayerCombat.instance.weaponsDrawn);
        DialogueLua.SetVariable("timeOfDay", DateTimeManager.instance.GetTimeOfDay());
        
        if (character != null) //Then start dialogue
        {
            currentConversant = character;
            UpdateNPCDialogueValues(character);
            DialogueManager.StartConversation(character.name, playerController.transform, actor);
        }
        else
        {
            DialogueManager.StartConversation(dialogueOverride, playerController.transform, actor);
        }

        ToggleCursor(true);

        HUD.ToggleAllActiveElements(true);
        dialogueMenuOpen = true;
        checkForInteract = false;
        interactAnim.SetBool("interact", false);
    }

    public void OnDialogueEnd()
    {
        if (currentConversant != null)
        {
            //update the character's playerAffinity for any changes that may have occurred during dialogue
            currentConversant.playerAffinity = DialogueLua.GetVariable("playerAffinity").asInt;
            //update journal to include new character
            JournalManager.OnCharacterInteraction_Static(currentConversant);
            currentConversant.hasMetPlayer = true;
            currentConversant = null;
        }
        HUD.ToggleAllActiveElements(false);
        ToggleCursor(false);
        checkForInteract = true;
        dialogueMenuOpen = false;
    }

    private void UpdateNPCDialogueValues(NPC character)
    {
        DialogueLua.SetVariable("firstInteraction", !character.hasMetPlayer);
        DialogueLua.SetVariable("playerAffinity", character.playerAffinity);
        DialogueLua.SetVariable("playerSpeaksLanguage", PlayerStats.instance.PlayerSpeaksLanguage(character.primaryLanguage));
        if (character.residence != null)
        {
            DialogueLua.SetVariable("localRep", character.residence.playerReputation);

            //I probably want to set some sort of weighted result for this, so they're not calling the player "pig rescuer" when they've saved the town
            //Probably some sort of weight based on the player's affinity with the NPC as well, lean towards more negative titles when they dislike the player
            DialogueLua.SetVariable("playerTitle", character.residence.GetRandomTitle());
        }
        if (character.faction != Faction.None)
        {
            //Will likely end up making this a SO as well...
        }
    }

    private void UpdateSpeechSkillScores()
    {
        DialogueLua.SetVariable("barter", PlayerStats.instance.statSheet.barter.GetValue());
        DialogueLua.SetVariable("enchantment", PlayerStats.instance.statSheet.enchantment.GetValue());
        DialogueLua.SetVariable("intuition", PlayerStats.instance.statSheet.intuition.GetValue());
        DialogueLua.SetVariable("leadership", PlayerStats.instance.statSheet.leadership.GetValue());
        DialogueLua.SetVariable("linguistics", PlayerStats.instance.statSheet.linguistics.GetValue());
        DialogueLua.SetVariable("speech", PlayerStats.instance.statSheet.speech.GetValue());
    }
    #endregion

    #region - Display Message -
    public void DisplayPopup(string message)
    {
        reminderText.text = message;
        popupAnim.Play("reminder");
    }

    public void AddNotification(string message)
    {
        displayMessages.Enqueue(message);
        JournalManager.AddLog_Static(message);

        if (displayBoxActive == false)
            StartCoroutine(NotificationQueue());
    }

    private IEnumerator NotificationQueue()
    {
        displayBoxActive = true;

        //add slight delay for when adding multiple items
        yield return new WaitForSecondsRealtime(0.1f);

        while (displayMessages.Count > 0)
        {
            displayText.text = displayMessages.Dequeue();

            if (displayMessages.Count >= 1)
            {
                displayText.text += "\n \n" + displayMessages.Dequeue();
            }
            if (displayMessages.Count >= 1)
            {
                displayText.text += "\n \n" + displayMessages.Dequeue();
            }

            displayAnim.Play("reminder");
            yield return new WaitForSecondsRealtime(messageDelay);
            yield return null;
        }
        displayBoxActive = false;
    }
    #endregion

    public void DisplayCraftingScreen(CraftingStation station)
    {
        craftingManager.gameObject.SetActive(true);
        craftingManager.DisplayRecipes(station.craftingType);

        ToggleCursor(true);
        checkForInteract = false;
        interactAnim.SetBool("interact", false);
    }

    public void CloseCraftingScreen()
    {
        craftingManager.gameObject.SetActive(false);
        playerController.isInteracting = false;

        ToggleCursor(false);
        checkForInteract = true;
    }

    public void DisplayQuestBoardScreen(QuestBoard board)
    {
        var anim = questBoardScreen.anim;
        anim.SetBool("displayQuest", false);
        anim.SetTrigger("open");
        questBoardScreen.DisplayAvailableQuests(board);
        OnMenuOpen();
    }
}