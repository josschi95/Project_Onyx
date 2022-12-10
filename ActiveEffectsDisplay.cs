using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveEffectsDisplay : MonoBehaviour
{
    Player_Settings playerSettings;

    [SerializeField] Transform activeEffectParent;
    [SerializeField] GameObject activeEffectElement;

    private void Start()
    {
        playerSettings = Player_Settings.instance;
        playerSettings.onSettingsChanged += OnSettingsChange;
    }

    void OnSettingsChange()
    {

    }

    private void OnNewEffect()
    {
        GameObject icon = Instantiate(activeEffectElement, activeEffectParent);
        //icon.GetComponent<Image>().sprite = 
    }
}
