using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConjuredCampfire : MonoBehaviour, IInteractable
{
    [SerializeField] Light lightSource;
    [SerializeField] AudioSource audioSource;
    [SerializeField] FireFlicker flicker;
    [Space]
    [SerializeField] ParticleSystem fire;
    [SerializeField] ParticleSystem fireflies;
    [SerializeField] ParticleSystem smoke;

    public float timeToTransition;
    private float lightIntensity = 2;
    private float audioIntensity = 1;

    private void Start()
    {
        lightSource.intensity = 0;
        audioSource.volume = 0;
        flicker.enabled = false;
        StartCoroutine(LightFire());
        StartCoroutine(DeactivateSelf());
    }

    public float interactionDistance = 2;
    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        return true;
    }

    public void Interact(CharacterController controller)
    {
        if (controller is not PlayerController) return;

        UIManager.instance.OnMenuOpen();

        var selection = UIManager.instance.selectionPanel;
        selection.gameObject.SetActive(true);

        selection.DisplayOptions("Sit", "Dismiss Campfire", "Cancel", "", "");
        selection.header.text = "Campfire";
        selection.button00.onClick.AddListener(delegate 
        { 
            Debug.Log("Sitting at campfire");
            fireflies.Play();
            selection.gameObject.SetActive(false);
            UIManager.instance.OnMenuClose();
        });
        selection.button01.onClick.AddListener(delegate 
        {
            StartCoroutine(SnuffOut());
            selection.gameObject.SetActive(false);
            UIManager.instance.OnMenuClose();
        });
        selection.button02.onClick.AddListener(delegate 
        { 
            selection.gameObject.SetActive(false);
            UIManager.instance.OnMenuClose();
        });
    }

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString("Use", transform.name, false);
    }

    private IEnumerator LightFire()
    {
        fire.Play();
        smoke.Stop();
        fireflies.Stop();

        float elapsedTime = 0;
        while (elapsedTime < timeToTransition)
        {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity, lightIntensity, (elapsedTime / timeToTransition));
            audioSource.volume = Mathf.Lerp(audioSource.volume, audioIntensity, (elapsedTime / timeToTransition));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        flicker.enabled = true;
    }


    private IEnumerator SnuffOut()
    {
        float elapsedTime = 0;

        fire.Stop(); //would prefer to change this to stop looping
        fireflies.Stop();
        smoke.Play();
        flicker.StopFlickering = false;
        while (elapsedTime < timeToTransition)
        {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity, 0, (elapsedTime / timeToTransition));
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, (elapsedTime / timeToTransition));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private IEnumerator DeactivateSelf()
    {
        while (Vector3.Distance(PlayerController.instance.transform.position, transform.position) < 20)
        {
            yield return null;
        }
        StartCoroutine(SnuffOut());
    }
}
