using TMPro;
using UnityEngine;
using UnityEngine.UI;


//Need to set max values for all sliders


public class SettingsMenu : MonoBehaviour
{
    private Player_Settings playerSettings;
    [SerializeField] private bool mainMenu = false;
    [Space]
    public Button saveChangesButton;

    [Header("General Settings")]
    public TMP_Text difficultyText;
    public TMP_Text textSpeedText, xSensText, ySensText, masterVolumText, fxVolumeText;
    public Slider difficultySlider, textSpeedSlider, xSensitivity, ySensitivity, masterVolumeSlider, fxVolumeSlider;
    public Toggle invertXToggle, invertYToggle;

    [Header("HUD Settings")]
    public TMP_Text crosshairText;
    public TMP_Text compassText, activeEffectsText, statBarText, quickslotText, NPC_HUDText;
    public Slider crosshairSlider, statbarSlider, quickslotSlider, compassSlider, activeEffectsSlider, NPC_HUDSlider;
    public Toggle displayStatNumToggle, displayQuestMarkerToggle, displayHUDToggle; 

    private void Start()
    {
        playerSettings = Player_Settings.instance;

        SetSliderBounds();
        SetInitialValues();

        saveChangesButton.onClick.AddListener(SaveChanges);

        #region - General Settings -
        //Separating this so I can re-use this script for the main menu
        if (mainMenu == false)
        {
            difficultySlider.onValueChanged.AddListener(delegate
            {
                if (difficultySlider.value == 0) difficultyText.text = "For Story";
                else if (difficultySlider.value == 1) difficultyText.text = "For Honor";
                else difficultyText.text = "For Glory";
            });
        }

        textSpeedSlider.onValueChanged.AddListener(delegate 
        { 
            textSpeedText.text = "Text Speed: " + textSpeedSlider.value;
        });

        xSensitivity.onValueChanged.AddListener(delegate
        {
            xSensText.text = "X Sensitivity: " + xSensitivity.value;
        });
        ySensitivity.onValueChanged.AddListener(delegate
        {
            ySensText.text = "Y Sensitivity: " + ySensitivity.value;
        });

        //Audio
        masterVolumeSlider.onValueChanged.AddListener(delegate
        {
            masterVolumText.text = "Master Volume: " + masterVolumeSlider.value;
        });
        fxVolumeSlider.onValueChanged.AddListener(delegate
        {
            fxVolumeText.text = "FX Volume: " + fxVolumeSlider.value;
        });
        #endregion

        #region - HUD Settings -
        crosshairSlider.onValueChanged.AddListener(delegate
        {
            if (crosshairSlider.value == 0) crosshairText.text = "Crosshair: Off";
            else if (crosshairSlider.value == 1) crosshairText.text = "Crosshair: Small";
            else if (crosshairSlider.value == 2) crosshairText.text = "Crosshair: Medium";
            else crosshairText.text = "Crosshair: Large";
        });

        statbarSlider.onValueChanged.AddListener(delegate
        {
            if (statbarSlider.value == 0) statBarText.text = "Statbar: Off";
            else if (statbarSlider.value == 1) statBarText.text = "Statbar: Small";
            else if (statbarSlider.value == 2) statBarText.text = "Statbar: Medium";
            else statBarText.text = "Statbar: Large";
        });

        compassSlider.onValueChanged.AddListener(delegate
        {
            if (compassSlider.value == 0) compassText.text = "Compass: Off";
            else if (compassSlider.value == 1) compassText.text = "Compass: Small";
            else if (compassSlider.value == 2) compassText.text = "Compass: Medium";
            else compassText.text = "Compass: Large";
        });

        quickslotSlider.onValueChanged.AddListener(delegate
        {
            if (quickslotSlider.value == 0) quickslotText.text = "Quickslots: Off";
            else if (quickslotSlider.value == 1) quickslotText.text = "Quickslots: Small";
            else if (quickslotSlider.value == 2) quickslotText.text = "Quickslots: Medium";
            else quickslotText.text = "Quickslots: Large";
        });

        activeEffectsSlider.onValueChanged.AddListener(delegate
        {
            if (activeEffectsSlider.value == 0) activeEffectsText.text = "Active Effects: Off";
            else if (activeEffectsSlider.value == 1) activeEffectsText.text = "Active Effects: Small";
            else if (activeEffectsSlider.value == 2) activeEffectsText.text = "Active Effects: Medium";
            else activeEffectsText.text = "Active Effects: Large";
        });

        NPC_HUDSlider.onValueChanged.AddListener(delegate
        {
            if (NPC_HUDSlider.value == 0) NPC_HUDText.text = "NPC HUDs: Off";
            else if (NPC_HUDSlider.value == 1) NPC_HUDText.text = "NPC HUDs: Single";
            else NPC_HUDText.text = "NPC HUDs: Individual";
        });
        #endregion
    }

    private void SaveChanges()
    {
        if (mainMenu == false)
            GameMaster.instance.SetDifficulty((int)difficultySlider.value); ;

        playerSettings.textSpeed = (int)textSpeedSlider.value;
        playerSettings.XTurnSensitivity = (int)xSensitivity.value;
        playerSettings.YTiltSensitivity = (int)ySensitivity.value;

        playerSettings.masterVolumeMultiplier = masterVolumeSlider.value;
        playerSettings.AFX_VolumMultiplier = fxVolumeSlider.value;

        playerSettings.displayHUD = displayHUDToggle.isOn;
        playerSettings.displayStatNumbers = displayStatNumToggle.isOn;
        playerSettings.displayQuestMarker = displayQuestMarkerToggle.isOn;

        playerSettings.invert_X = invertXToggle.isOn;
        playerSettings.invert_Y = invertYToggle.isOn;

        //

        playerSettings.crosshair = (int)crosshairSlider.value;
        playerSettings.statBars = (int)statbarSlider.value;
        playerSettings.compass = (int)compassSlider.value;
        playerSettings.quickslots = (int)quickslotSlider.value;
        playerSettings.activeEffects = (int)activeEffectsSlider.value;
        playerSettings.npcHUD = (int)NPC_HUDSlider.value;

        playerSettings.displayHUD = displayHUDToggle.isOn;
        playerSettings.displayStatNumbers = displayStatNumToggle.isOn;
        playerSettings.displayQuestMarker = displayQuestMarkerToggle.isOn;

        playerSettings.OnGeneralSettingsChanged();

        UIManager.instance.AddNotification("Changes Saved");
    }

    private void SetSliderBounds()
    {
        #region - General Settings -
        //Difficulty
        if (mainMenu == false)
        {
            difficultySlider.minValue = 0;
            difficultySlider.maxValue = 2;
            difficultySlider.wholeNumbers = true;
        }

        //Text Speed
        textSpeedSlider.minValue = 25;
        textSpeedSlider.maxValue = 100;
        textSpeedSlider.wholeNumbers = true;
        //X Sensitivity
        xSensitivity.minValue = 1;
        xSensitivity.maxValue = 20;
        xSensitivity.wholeNumbers = true;
        //Y Sensitivity
        ySensitivity.minValue = 1;
        ySensitivity.maxValue = 20;
        ySensitivity.wholeNumbers = true;
        //Master Volume
        masterVolumeSlider.minValue = 0;
        masterVolumeSlider.maxValue = 100;
        masterVolumeSlider.wholeNumbers = true;
        //FX Volume
        fxVolumeSlider.minValue = 0;
        fxVolumeSlider.maxValue = 100;
        fxVolumeSlider.wholeNumbers = true;
        #endregion

        #region - HUD Settings -
        crosshairSlider.minValue = 0;
        crosshairSlider.maxValue = 3;
        crosshairSlider.wholeNumbers = true;
        //
        statbarSlider.minValue = 0;
        statbarSlider.maxValue = 3;
        statbarSlider.wholeNumbers = true;
        //
        quickslotSlider.minValue = 0;
        quickslotSlider.maxValue = 3;
        quickslotSlider.wholeNumbers = true;
        //
        compassSlider.minValue = 0;
        compassSlider.maxValue = 3;
        compassSlider.wholeNumbers = true;
        //
        activeEffectsSlider.minValue = 0;
        activeEffectsSlider.maxValue = 3;
        activeEffectsSlider.wholeNumbers = true;
        //
        NPC_HUDSlider.minValue = 0;
        NPC_HUDSlider.maxValue = 2;
        #endregion
    }

    public void SetInitialValues()
    {
        #region - General Settings -
        //Difficulty
        if (mainMenu == false)
        {
            difficultySlider.value = GameMaster.instance.difficultyLevel;
            if (difficultySlider.value == 0) difficultyText.text = "For Story";
            else if (difficultySlider.value == 1) difficultyText.text = "For Honor";
            else difficultyText.text = "For Glory";
        }

        //Text Speed
        textSpeedSlider.value = playerSettings.textSpeed;
        textSpeedText.text = "Text Speed: " + textSpeedSlider.value;

        //X Sensitivity
        xSensitivity.value = playerSettings.XTurnSensitivity;
        xSensText.text = "X Sensitivity: " + xSensitivity.value;

        //Y Sensitivity
        ySensitivity.value = playerSettings.YTiltSensitivity;
        ySensText.text = "Y Sensitivity: " + ySensitivity.value;

        //Master Volume
        masterVolumeSlider.value = playerSettings.masterVolumeMultiplier;
        masterVolumText.text = "Master Volume: " + masterVolumeSlider.value;

        //FX Volume
        fxVolumeSlider.value = playerSettings.AFX_VolumMultiplier;
        fxVolumeText.text = "FX Volume: " + fxVolumeSlider.value;

        invertXToggle.isOn = playerSettings.invert_X;
        invertYToggle.isOn = playerSettings.invert_Y;
        #endregion

        #region - HUD Settings -
        //Crosshair
        crosshairSlider.value = playerSettings.crosshair;
        if (crosshairSlider.value == 0) crosshairText.text = "Crosshair: Off";
        else if (crosshairSlider.value == 1) crosshairText.text = "Crosshair: Small";
        else if (crosshairSlider.value == 2) crosshairText.text = "Crosshair: Medium";
        else crosshairText.text = "Crosshair: Large";
        //Statbar
        statbarSlider.value = playerSettings.statBars; ;
        if (statbarSlider.value == 0) statBarText.text = "Statbar: Off";
        else if (statbarSlider.value == 1) statBarText.text = "Statbar: Small";
        else if (statbarSlider.value == 2) statBarText.text = "Statbar: Medium";
        else statBarText.text = "Statbar: Large";
        //Compass
        compassSlider.value = playerSettings.compass;
        if (compassSlider.value == 0) compassText.text = "Compass: Off";
        else if (compassSlider.value == 1) compassText.text = "Compass: Small";
        else if (compassSlider.value == 2) compassText.text = "Compass: Medium";
        else compassText.text = "Compass: Large";
        //Quickslots
        quickslotSlider.value = playerSettings.quickslots;
        if (quickslotSlider.value == 0) quickslotText.text = "Quickslots: Off";
        else if (quickslotSlider.value == 1) quickslotText.text = "Quickslots: Small";
        else if (quickslotSlider.value == 2) quickslotText.text = "Quickslots: Medium";
        else quickslotText.text = "Quickslots: Large";
        //Activeeffects
        activeEffectsSlider.value = playerSettings.activeEffects;
        if (activeEffectsSlider.value == 0) activeEffectsText.text = "Active Effects: Off";
        else if (activeEffectsSlider.value == 1) activeEffectsText.text = "Active Effects: Small";
        else if (activeEffectsSlider.value == 2) activeEffectsText.text = "Active Effects: Medium";
        else activeEffectsText.text = "Active Effects: Large";
        //NPC HUDs
        NPC_HUDSlider.value = playerSettings.npcHUD;
        if (NPC_HUDSlider.value == 0) NPC_HUDText.text = "NPC HUDs: Off";
        else if (NPC_HUDSlider.value == 1) NPC_HUDText.text = "NPC HUDs: Single";
        else NPC_HUDText.text = "NPC HUDs: Individual";

        displayHUDToggle.isOn = playerSettings.displayHUD;
        displayStatNumToggle.isOn = playerSettings.displayStatNumbers;
        displayQuestMarkerToggle.isOn = playerSettings.displayQuestMarker;
        #endregion
    }

    public void SaveGame()
    {
        Debug.Log("Saving Game");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }
}
