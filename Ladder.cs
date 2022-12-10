using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    public float rotation;
    public Transform center;
    public Transform ladderBottom;
    public Transform ladderTop;

    public float interactionDistance = 2;

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance && beingClimbed == false) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        return true;
    }

    public void Interact(CharacterController controller)
    {
        if (controller is PlayerController)
            StartCoroutine(PlayerController.instance.ClimbingLadder(this));

        Debug.Log("I might be able to get NPCs to use ladders now");
    }

    [HideInInspector] public bool beingClimbed;
    public string interactionMethod = "Climb";
    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, transform.name, false);
    }

    private IEnumerator ToggleLadderCollider()
    {
        GetComponent<Collider>().isTrigger = true;
        while (beingClimbed == true)
        {
            yield return null;
        }
        GetComponent<Collider>().isTrigger = false;
    }
}