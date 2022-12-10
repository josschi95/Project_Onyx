using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayCameraAdjust : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.ReadjustCameraAngle();
            }
        }
    }
}

//Lerp turn.x down to 0 if greater