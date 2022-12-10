using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class Player_Settings : MonoBehaviour
{
    #region - Singleton -
    public static Player_Settings instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerSettings found");
            return;
        }

        instance = this;
    }
    #endregion

    //Callback for when settings are changed
    public delegate void OnSettingsChanged();
    public OnSettingsChanged onSettingsChanged;

    #region - Player General Info -
    [HideInInspector] public string playerBirthplace;
    [HideInInspector] public int playerAge;
    [HideInInspector] public string playerVirtues;
    [HideInInspector] public string playerVices;
    [HideInInspector] public string playerDowntime;
    #endregion

    public UnityUITypewriterEffect typewriter;

    [Header("Controls")]
    [Range(1, 20)] public int XTurnSensitivity = 10; //not fully implemented
    [Range(1, 20)] public int YTiltSensitivity = 10; //not fully implemented
    public bool invert_X = false;
    public bool invert_Y = false;

    [Header("Audio")]
    [Range(0, 100)] public float masterVolumeMultiplier = 100; //not implemented
    [Range(0, 100)] public float AFX_VolumMultiplier = 100; //not implemented

    public bool displayHUD = true;
    public bool displayStatNumbers = true;
    public bool displayQuestMarker = true;

    [Range(25, 100)] public int textSpeed = 50;

    //off, small, medium, large
    [Range(0, 3)] public int statBars = 1;
    [Range(0, 3)] public int quickslots = 1;
    [Range(0, 3)] public int crosshair = 1;
    [Range(0, 3)] public int activeEffects = 1;
    [Range(0, 3)] public int compass = 1;
    //Off, OnHud, OverNPC
    [Range(0,2)] public int npcHUD = 1;

    public bool useDefaultAttack;
    //0: Default 1: Movement 2: Look
    [Range(0, 2)] public int attackDirection;

    private void Start()
    {
        GetSettings();

        if (typewriter == null)
        {
            //Debug.Log("Need to find some simple way to locate this");
        }
        else typewriter.charactersPerSecond = textSpeed;
       
        if (onSettingsChanged != null) onSettingsChanged.Invoke();
    }

    [ContextMenu("Reset All Settings")]
    private void ResetAllSettings()
    {
        PlayerPrefs.DeleteAll();

        //General
        PlayerPrefs.SetInt("textSpeed", 50);
        PlayerPrefs.SetInt("XTurnSensitivity", 10);
        PlayerPrefs.SetInt("YTiltSensitivity", 10);
        PlayerPrefs.SetInt("invert_X", 0);
        PlayerPrefs.SetInt("invert_Y", 0);

        PlayerPrefs.SetFloat("masterVolumeMultiplier", 100);
        PlayerPrefs.SetFloat("AFX_VolumMultiplier", 100);

        //HUD
        PlayerPrefs.SetInt("statBars", 1);
        PlayerPrefs.SetInt("quickslots", 1);
        PlayerPrefs.SetInt("crosshair", 1);
        PlayerPrefs.SetInt("activeEffects", 1);
        PlayerPrefs.SetInt("compass", 1);
        PlayerPrefs.SetInt("npcHUD", 1);

        PlayerPrefs.SetInt("displayHUD", 1);
        PlayerPrefs.SetInt("displayStatNumbers", 1);
        PlayerPrefs.SetInt("displayQuestMarker", 1);

        PlayerPrefs.Save();
        GetSettings();
    }

    public void ToggleHUD(bool showHUD)
    {
        displayHUD = showHUD;
        SaveSettings();
        if (onSettingsChanged != null) onSettingsChanged.Invoke();
    }

    public void OnGeneralSettingsChanged()
    {
        SaveSettings();
        typewriter.charactersPerSecond = textSpeed;
        if (onSettingsChanged != null) onSettingsChanged.Invoke();
    }

    [ContextMenu("Save Settings")]
    public void SaveSettings()
    {
        //General
        PlayerPrefs.SetInt("textSpeed", textSpeed);
        PlayerPrefs.SetInt("XTurnSensitivity", XTurnSensitivity);
        PlayerPrefs.SetInt("YTiltSensitivity", YTiltSensitivity);
        PlayerPrefs.SetInt("invert_X", BoolToInt(invert_X));
        PlayerPrefs.SetInt("invert_Y", BoolToInt(invert_Y));

        PlayerPrefs.SetFloat("masterVolumeMultiplier", masterVolumeMultiplier);
        PlayerPrefs.SetFloat("AFX_VolumMultiplier", AFX_VolumMultiplier);

        //HUD
        PlayerPrefs.SetInt("statBars", statBars);
        PlayerPrefs.SetInt("quickslots", quickslots);
        PlayerPrefs.SetInt("crosshair", crosshair);
        PlayerPrefs.SetInt("activeEffects", activeEffects);
        PlayerPrefs.SetInt("compass", compass);
        PlayerPrefs.SetInt("npcHUD", npcHUD);

        PlayerPrefs.SetInt("displayHUD", BoolToInt(displayHUD));
        PlayerPrefs.SetInt("displayStatNumbers", BoolToInt(displayStatNumbers));
        PlayerPrefs.SetInt("displayQuestMarker", BoolToInt(displayQuestMarker));

        PlayerPrefs.Save();
    }

    public void GetSettings()
    {
        //General
        textSpeed = PlayerPrefs.GetInt("textSpeed");
        XTurnSensitivity = PlayerPrefs.GetInt("XTurnSensitivity");
        YTiltSensitivity = PlayerPrefs.GetInt("YTiltSensitivity");
        invert_X = IntToBool(PlayerPrefs.GetInt("invert_X"));
        invert_Y = IntToBool(PlayerPrefs.GetInt("invert_Y"));

        masterVolumeMultiplier = PlayerPrefs.GetFloat("masterVolumeMultiplier");
        AFX_VolumMultiplier = PlayerPrefs.GetFloat("AFX_VolumMultiplier");

        //HUD
        statBars = PlayerPrefs.GetInt("statBars");
        quickslots = PlayerPrefs.GetInt("quickslots");
        crosshair = PlayerPrefs.GetInt("crosshair");
        activeEffects = PlayerPrefs.GetInt("activeEffects");
        compass = PlayerPrefs.GetInt("compass");
        npcHUD = PlayerPrefs.GetInt("npcHUD");

        displayHUD = IntToBool(PlayerPrefs.GetInt("displayHUD"));
        displayStatNumbers = IntToBool(PlayerPrefs.GetInt("displayStatNumbers"));
        displayQuestMarker = IntToBool(PlayerPrefs.GetInt("displayQuestMarker"));
    }

    private int BoolToInt(bool value)
    {
        if (value) return 1;
        else return 0;
    }

    private bool IntToBool(int value)
    {
        if (value != 0) return true;
        else return false;
    }
}