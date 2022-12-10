using System.Collections;
using UnityEngine;

public class InvestigateState : _State
{
    public float returnToStartTime = 8f;
    private bool returnToPreviousBehavior = false;
    private bool resetDelay = false;
    private Coroutine delayCoroutine;

    public override _State OnStateEnter(NPCController controller)
    {
        controller.charDetect.ToggleDetection(controller.isHostile);

        resetDelay = false;
        returnToPreviousBehavior = false;
        return this;
    }

    //Investigating a sound, either player footsteps, a projectile hitting a surface, or an ability to throw sound
    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        if (controller.isHostile)
        {
            //If a target is found, switch to combat
            if (controller.target != null) return handler.combatIdle;
        }

        if (returnToPreviousBehavior == true) return controller.ResetToDefault();

        controller.UpdateAgentDestination(controller.soundSourcePosition);
        controller.FaceTarget(controller.soundSourcePosition);

        //The source has been investigated and nothing was found, return to previous behaviour
        //Later add some functionality for detecting dead bodies
        if (Vector3.Distance(controller.transform.position, controller.soundSourcePosition) <= 1)
        {
            //Add coroutine or function set to rotate left and right to scan the area
            if (resetDelay == false)
            {
                if (delayCoroutine != null) StopCoroutine(delayCoroutine);
                delayCoroutine = StartCoroutine(ResetDelay());
            }
        }

        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        resetDelay = false;
        returnToPreviousBehavior = false;
        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);
    }

    private IEnumerator ResetDelay()
    {
        resetDelay = true;
        yield return new WaitForSeconds(returnToStartTime);
        returnToPreviousBehavior = true;
        resetDelay = false;
    }
}

//Ideally, there would be a way to implement a counter for number of sound sources heard,
//and if it reaches a high enough number, then the NPC won't return to idle, and will remain in a combat-ready search state
