using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform outPosition;
    [SerializeField] private Portal linkedPortal;
    [Space]
    public ParticleSystem teleportFromFX;
    public ParticleSystem teleportToFX;
    [Space]
    public AudioSource audioSource;
    public AudioClip teleportFromClip;
    public AudioClip teleportToClip;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                if (teleportFromFX != null) teleportFromFX.Play();
                audioSource.clip = teleportFromClip;
                audioSource.Play();

                CameraHelper.instance.thirdPersonFollow_vcam.gameObject.SetActive(false);
                player.transform.position = linkedPortal.outPosition.position;
                player.transform.rotation = linkedPortal.outPosition.rotation;
                player.followTarget.transform.rotation = linkedPortal.outPosition.rotation;
                StartCoroutine(WarpDelay());
            }
        }
    }

    IEnumerator WarpDelay()
    {
        yield return new WaitForSeconds(1);
        if (linkedPortal.teleportToFX != null) linkedPortal.teleportToFX.Play();
        CameraHelper.instance.thirdPersonFollow_vcam.gameObject.SetActive(true);
        linkedPortal.audioSource.clip = teleportToClip;
        linkedPortal.audioSource.Play();
    }
}
