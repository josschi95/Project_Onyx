using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputHandler : MonoBehaviour
{
    #region - Singleton -
    private static InputHandler instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] private PlayerInput playerInput;
    private PlayerEquipmentManager playerEquip;
    private PlayerController playerController;
    private PlayerSpellcasting spellcasting;
    private QuickslotManager quickslots;
    private PlayerCombat playerCombat;
    private UIManager UIManager;
    private CameraHelper cam;
    private HUDManager HUD;
    private Player_Settings settings;

    private bool altClick;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerEquip = GetComponent<PlayerEquipmentManager>();
        playerController = GetComponent<PlayerController>();
        spellcasting = GetComponent<PlayerSpellcasting>();
        playerCombat = GetComponent<PlayerCombat>();
        UIManager = UIManager.instance;
        cam = CameraHelper.instance;
        HUD = HUDManager.instance;
        quickslots = HUD.quickslotManager;
        settings = Player_Settings.instance;
    }

    public void OnEnable()
    {
        playerInput.actions["Look"].performed += i => playerController.cameraLook = i.ReadValue<Vector2>();

        #region - Movement -
        playerInput.actions["Move"].performed += i => playerController.movementInput = i.ReadValue<Vector2>();
        playerInput.actions["Jump"].performed += i => playerController.OnJump();
        playerInput.actions["Crouch"].performed += i => playerController.OnCrouch(!playerController.isCrouching);
        playerInput.actions["Sprint"].performed += i => playerController.OnSprint(!playerController.isSprinting);
        playerInput.actions["Toggle Walk"].performed += i => playerController.isWalking = !playerController.isWalking;
        playerInput.actions["Change FOV"].performed += i => cam.CameraZoom(i.ReadValue<Vector2>());
        playerInput.actions["Change View"].performed += i => cam.CycleActiveCamera();
        #endregion

        #region - Character Controls -
        playerInput.actions["Interact"].performed += i => UIManager.Interact();
        playerInput.actions["Alternate Action"].started += i => altClick = true;
        playerInput.actions["Alternate Action"].canceled += i => altClick = false;

        playerInput.actions["Main Hand"].performed += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Primary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Primary();
            }
        };
        playerInput.actions["Main Hand"].canceled += context => playerCombat.Release_Primary();

        playerInput.actions["Off Hand"].performed += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Secondary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Secondary();
            }
        };
        playerInput.actions["Off Hand"].canceled += context => playerCombat.Release_Secondary();

        playerInput.actions["Cast Spell"].performed += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                spellcasting.PrepareSpell();
            }
            else if (context.interaction is PressInteraction)
            {
                spellcasting.QuickCast();
            }
        };
        playerInput.actions["Cast Spell"].canceled += context => spellcasting.ReleaseSpell();

        playerInput.actions["Ready Weapons"].performed += i => 
        {
            playerCombat.ReadyWeapons();
            UIManager.ContainerTakeAll();
        };
        playerInput.actions["Swap Weapons"].performed += i => playerEquip.SwapWeaponSets(!playerEquip.usingSecondaryWeaponSet, true);
        #endregion

        #region - Menu Controls -
        playerInput.actions["System Menu"].performed += i => UIManager.ToggleMenu_System();
        playerInput.actions["Open Menu"].performed += i => UIManager.TogglePlayerMenu();
        playerInput.actions["Character Menu"].performed += i => UIManager.ToggleMenu_Character();
        playerInput.actions["Inventory"].performed += i => UIManager.ToggleMenu_Inventory();
        playerInput.actions["Journal"].performed += i => UIManager.ToggleMenu_Journal();
        playerInput.actions["Map"].performed += i => UIManager.ToggleMenu_Map();
        playerInput.actions["Toggle HUD"].performed += i => settings.ToggleHUD(!settings.displayHUD);
        #endregion

        #region - Quickslots -
        playerInput.actions["Use Potion"].started += i => quickslots.potionSlot.OnUse();
        playerInput.actions["Use Ability"].started += i => quickslots.abilitySlot.OnUse();
        playerInput.actions["Use Relic"].started += i => quickslots.relicSlot.OnUse();

        playerInput.actions["Quickslot Use"].started += i => quickslots.UseActiveSlot();

        playerInput.actions["Quickslot Up"].started += i => quickslots.CycleActiveSlot(true);
        playerInput.actions["Quickslot Down"].started += i => quickslots.CycleActiveSlot(false);
        playerInput.actions["Quickslot Left"].started += i => quickslots.OnActiveSlotChange(false);
        playerInput.actions["Quickslot Right"].started += i => quickslots.OnActiveSlotChange(true);
        #endregion
    }

    public void OnDisable()
    {
        playerInput.actions["Look"].performed -= i => playerController.cameraLook = i.ReadValue<Vector2>();

        #region - Movement -
        playerInput.actions["Move"].performed -= i => playerController.movementInput = i.ReadValue<Vector2>();
        playerInput.actions["Jump"].performed -= i => playerController.OnJump();
        playerInput.actions["Crouch"].performed -= i => playerController.OnCrouch(!playerController.isCrouching);
        playerInput.actions["Sprint"].performed -= i => playerController.OnSprint(!playerController.isSprinting);
        playerInput.actions["Toggle Walk"].performed -= i => playerController.isWalking = !playerController.isWalking;
        playerInput.actions["Change FOV"].performed -= i => cam.CameraZoom(i.ReadValue<Vector2>());
        playerInput.actions["Change View"].performed -= i => cam.CycleActiveCamera();
        #endregion

        #region - Character Controls -
        playerInput.actions["Interact"].performed -= i => UIManager.Interact();
        playerInput.actions["Alternate Action"].started -= i => altClick = true;
        playerInput.actions["Alternate Action"].canceled -= i => altClick = false;

        playerInput.actions["Main Hand"].performed -= context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Primary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Primary();
            }
        };
        playerInput.actions["Main Hand"].canceled -= context => playerCombat.Release_Primary();

        playerInput.actions["Off Hand"].performed -= context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Secondary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Secondary();
            }
        };
        playerInput.actions["Off Hand"].canceled -= context => playerCombat.Release_Secondary();

        playerInput.actions["Cast Spell"].performed -= context =>
        {
            if (context.interaction is HoldInteraction)
            {
                spellcasting.PrepareSpell();
            }
            else if (context.interaction is PressInteraction)
            {
                spellcasting.QuickCast();
            }
        };
        playerInput.actions["Cast Spell"].canceled -= context => spellcasting.ReleaseSpell();

        playerInput.actions["Ready Weapons"].performed -= i =>
        {
            playerCombat.ReadyWeapons();
            UIManager.ContainerTakeAll();
        };
        playerInput.actions["Swap Weapons"].performed -= i => playerEquip.SwapWeaponSets(!playerEquip.usingSecondaryWeaponSet, true);
        #endregion

        #region - Menu Controls -
        playerInput.actions["System Menu"].performed -= i => UIManager.ToggleMenu_System();
        playerInput.actions["Open Menu"].performed -= i => UIManager.TogglePlayerMenu();
        playerInput.actions["Character Menu"].performed -= i => UIManager.ToggleMenu_Character();
        playerInput.actions["Inventory"].performed -= i => UIManager.ToggleMenu_Inventory();
        playerInput.actions["Journal"].performed -= i => UIManager.ToggleMenu_Journal();
        playerInput.actions["Map"].performed -= i => UIManager.ToggleMenu_Map();
        playerInput.actions["Toggle HUD"].performed -= i => settings.ToggleHUD(!settings.displayHUD);
        #endregion

        #region - Quickslots -
        playerInput.actions["Use Potion"].started -= i => quickslots.potionSlot.OnUse();
        playerInput.actions["Use Ability"].started -= i => quickslots.abilitySlot.OnUse();
        playerInput.actions["Use Relic"].started -= i => quickslots.relicSlot.OnUse();

        playerInput.actions["Quickslot Use"].started -= i => quickslots.UseActiveSlot();

        playerInput.actions["Quickslot Up"].started -= i => quickslots.CycleActiveSlot(true);
        playerInput.actions["Quickslot Down"].started -= i => quickslots.CycleActiveSlot(false);
        playerInput.actions["Quickslot Left"].started -= i => quickslots.OnActiveSlotChange(false);
        playerInput.actions["Quickslot Right"].started -= i => quickslots.OnActiveSlotChange(true);
        #endregion
    }

    public static bool AlternateClick_Static()
    {
        return instance.AlternateClick();
    }

    private bool AlternateClick()
    {
        return altClick;
    }

    public static Vector2 GetMousePosition_Static()
    {
        return instance.GetMousePosition();
    }
    
    private Vector2 GetMousePosition()
    {
        return playerInput.actions["Mouse Position"].ReadValue<Vector2>();
    }

    public static void TogglePlayerInput_Static(bool enabled)
    {
        instance.TogglePlayerInput(enabled);
    }

    private void TogglePlayerInput(bool enabled)
    {
        playerInput.enabled = enabled;
    }

    private Vector2 GetMouseDelta()
    {
        return playerInput.actions["Look"].ReadValue<Vector2>();
        //return playerControls.Player.Look.ReadValue<Vector2>();
    }
}

/*
playerControls.Player.Movement.performed += inputActions => playerController.movementInput = inputActions.ReadValue<Vector2>();
playerControls.Player.Look.performed += i => playerController.cameraLook = i.ReadValue<Vector2>();
playerControls.Player.MousePosition.performed += i => playerController.mousePosition = i.ReadValue<Vector2>();
playerControls.Player.FOVZoom.performed += i => cam.CameraZoom(i.ReadValue<Vector2>());
playerControls.Player.ChangeView.performed += i => cam.CycleActiveCamera();
playerControls.Player.Interact.started += context => UIManager.Interact();
playerControls.Player.Menu.started += context => UIManager.TogglePlayerMenu();
playerControls.Player.CharacterMenu.started += context => UIManager.ToggleMenu_Character();
playerControls.Player.InventoryMenu.started += context => UIManager.ToggleMenu_Inventory();
playerControls.Player.JournalMenu.started += context => UIManager.ToggleMenu_Journal();
playerControls.Player.MapMenu.started += context => UIManager.ToggleMenu_Map();
playerControls.Player.SystemMenu.started += context => UIManager.ToggleMenu_System();
playerControls.Player.SwapWeaponSet.started += context => playerEquip.SwapWeaponSets(!playerEquip.usingSecondaryWeaponSet);

playerControls.Player.AlternateClick.started += context => altClick = true;
playerControls.Player.AlternateClick.canceled += context => altClick = false;


playerControls.Player.CycleSlotLeft.started += _ => { quickslots.OnActiveSlotChange(false); };
playerControls.Player.CycleSlotRight.started += _ => { quickslots.OnActiveSlotChange(true); };
playerControls.Player.ActiveSlotDown.started += _ => { quickslots.CycleActiveSlot(false); };
playerControls.Player.ActiveSlotUp.started += _ => { quickslots.CycleActiveSlot(true); };

playerControls.Player.UseActiveSlot.started += _ => { quickslots.UseActiveSlot(); };

playerControls.Player.UseAbility.started += _ => { quickslots.abilitySlot.OnUse(); };
playerControls.Player.UsePotion.started += _ => { quickslots.potionSlot.OnUse(); };
playerControls.Player.UseRelic.started += _ => { quickslots.relicSlot.OnUse(); };

playerControls.Player.Jump.performed += _ => playerController.OnJump();
playerControls.Player.Crouch.started += _ => playerController.OnCrouch(!playerController.isCrouching);
//playerControls.Player.Dodge.performed += _ => playerController.OnDodge();
playerControls.Player.ToggleWalk.started += context => playerController.isWalking = !playerController.isWalking;
playerControls.Player.Sprint.started += context => playerController.OnSprint(!playerController.isSprinting);
playerControls.Player.Ready.performed += _ =>
{
    playerCombat.ReadyWeapons();
    UIManager.ContainerTakeAll();
};


playerControls.Player.MainHand.performed += context =>
{
    if (context.interaction is HoldInteraction)
    {
        playerCombat.Hold_Primary();
    }
    else if (context.interaction is PressInteraction)
    {
        playerCombat.Press_Primary();
    }
};
playerControls.Player.MainHand.canceled += context => playerCombat.Release_Primary();

playerControls.Player.OffHand.performed += context =>
{
    if (context.interaction is HoldInteraction)
    {
        playerCombat.Hold_Secondary();
    }
    else if (context.interaction is PressInteraction)
    {
        playerCombat.Press_Secondary();
    }
};
playerControls.Player.OffHand.canceled += context => playerCombat.Release_Secondary();


        //playerControls.Disable();

        #region - Player -
        playerControls.Player.Movement.performed -= inputActions => playerController.movementInput = inputActions.ReadValue<Vector2>();
        playerControls.Player.Look.performed -= i => playerController.cameraLook = i.ReadValue<Vector2>();
        playerControls.Player.MousePosition.performed -= i => playerController.mousePosition = i.ReadValue<Vector2>();
        playerControls.Player.FOVZoom.performed -= i => cam.CameraZoom(i.ReadValue<Vector2>());
        playerControls.Player.ChangeView.performed -= i => cam.CycleActiveCamera();
        playerControls.Player.Interact.started -= context => UIManager.Interact();
        playerControls.Player.Menu.started -= context => UIManager.TogglePlayerMenu();
        playerControls.Player.CharacterMenu.started -= context => UIManager.ToggleMenu_Character();
        playerControls.Player.InventoryMenu.started -= context => UIManager.ToggleMenu_Inventory();
        playerControls.Player.JournalMenu.started -= context => UIManager.ToggleMenu_Journal();
        playerControls.Player.MapMenu.started -= context => UIManager.ToggleMenu_Map();
        playerControls.Player.SystemMenu.started -= context => UIManager.ToggleMenu_System();
        playerControls.Player.SwapWeaponSet.started -= context => playerEquip.SwapWeaponSets(!playerEquip.usingSecondaryWeaponSet);

        playerControls.Player.AlternateClick.started -= context => altClick = true;
        playerControls.Player.AlternateClick.canceled -= context => altClick = false;

        #region - Hotkeys -
        playerControls.Player.UseActiveSlot.started -= _ => { quickslots.UseActiveSlot(); };

        playerControls.Player.CycleSlotLeft.started -= _ => { quickslots.OnActiveSlotChange(false); };
        playerControls.Player.CycleSlotRight.started -= _ => { quickslots.OnActiveSlotChange(true); };
        playerControls.Player.ActiveSlotDown.started -= _ => { quickslots.CycleActiveSlot(false); };
        playerControls.Player.ActiveSlotUp.started -= _ => { quickslots.CycleActiveSlot(true); };

        playerControls.Player.UseAbility.started -= _ => { quickslots.abilitySlot.OnUse(); };
        playerControls.Player.UsePotion.started -= _ => { quickslots.potionSlot.OnUse(); };
        playerControls.Player.UseRelic.started -= _ => { quickslots.relicSlot.OnUse(); };
        #endregion

        #endregion

        #region - Land -
        playerControls.Player.Jump.performed -= _ => playerController.OnJump();
        playerControls.Player.Crouch.started -= _ => playerController.OnCrouch(!playerController.isCrouching);
        //playerControls.Land.Dodge.performed -= _ => playerController.OnDodge();
        playerControls.Player.ToggleWalk.started -= context => playerController.isWalking = !playerController.isWalking;
        playerControls.Player.Sprint.started -= context => playerController.OnSprint(!playerController.isSprinting);
        playerControls.Player.Ready.performed -= _ =>
        {
            playerCombat.ReadyWeapons();
            UIManager.ContainerTakeAll();
        };

        playerControls.Player.MainHand.performed -= context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Primary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Primary();
            }
        };
        playerControls.Player.MainHand.canceled -= context => playerCombat.Release_Primary();

        playerControls.Player.OffHand.performed -= context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Secondary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Secondary();
            }
        };
        playerControls.Player.OffHand.canceled -= context => playerCombat.Release_Secondary();
        #endregion

*/