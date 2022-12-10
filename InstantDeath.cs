using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InstantDeath : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera deathCam;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            CharacterStats character = other.GetComponent<CharacterStats>();
            if (character != null)
            {
                character.Die();
                if (character is PlayerStats player)
                {
                    //UIManager.instance.DisplayDeathMessage("You Plummeted to Your Death");
                    //Vector3 contactPosition = player.transform.position;
                    //contactPosition += new Vector3(0, 10, 0);
                    //deathCam.transform.position = contactPosition;
                    //deathCam.transform.rotation = Quaternion.Euler(90f, PlayerController.instance.turn.x, 0f);
                    //deathCam.gameObject.SetActive(true);
                }
            }
        }
    }
}
