using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour, IInteractable
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Light lightSource;
    [Space]
    [SerializeField] private bool isLit;
    [Space]
    public float interactionDistance = 2;
    public string lightSourceName;
    public string interactionMethod = "Light";
    [Space]
    [SerializeField] private bool swapModel;
    [SerializeField] GameObject litModel;
    [SerializeField] GameObject unlitModel;

    private void Start()
    {
        ToggleLight(isLit);
    }

    public bool CanBeAccessed(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, lightSourceName, false);
    }

    public void Interact(CharacterController controller)
    {
        ToggleLight(!isLit);
    }

    private void ToggleLight(bool light)
    {
        //Extinguish the Flame
        if (light == false)
        {
            isLit = false;
            interactionMethod = "Light";
            if (lightSource != null) lightSource.enabled = false;
            particles.Stop();
            if (swapModel) SwapModels(false);
        }
        //Light the flame
        else
        {
            isLit = true;
            interactionMethod = "Extinguish";
            if (lightSource != null) lightSource.enabled = true;
            particles.Play();
            if (swapModel) SwapModels(true);
        }
    }

    private void SwapModels(bool light)
    {
        if (light)
        {
            litModel.SetActive(true);
            unlitModel.SetActive(false);
        }
        else
        {
            litModel.SetActive(false);
            unlitModel.SetActive(true);
        }
    }
}
