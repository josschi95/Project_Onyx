using System.Collections;
using UnityEngine;

public class SearchState : _State
{
    public float returnToStartTime = 8f;
    private bool returnToPreviousBehavior = false;
    private bool resetDelay = false;
    private Coroutine delayCoroutine;

    public override _State OnStateEnter(NPCController controller)
    {
        resetDelay = false;
        returnToPreviousBehavior = false;
        controller.charDetect.ToggleDetection(controller.isHostile);

        controller.agent.speed = controller.runSpeed;
        controller.agent.stoppingDistance = controller.defaultStoppingDistance;
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        if (controller.isHostile)
        {
            //If a target is found, switch to combat
            if (controller.target != null) return handler.combatIdle;

            //No target is found, but a sound is heard, switch to investigating
            else if (controller.soundDetected == true) return handler.investigate;
        }

        if (returnToPreviousBehavior == true) return controller.ResetToDefault();

        if (controller.lastTargetPosition != Vector3.zero)
        {
            controller.UpdateAgentDestination(controller.lastTargetPosition);
            controller.FaceTarget(controller.lastTargetPosition);
            if (Vector3.Distance(controller.transform.position, controller.lastTargetPosition) > controller.agent.stoppingDistance)
            {
                //Add coroutine or function set to rotate left and right to scan the area
                if (resetDelay == false)
                {
                    if (delayCoroutine != null) StopCoroutine(delayCoroutine);
                    delayCoroutine = StartCoroutine(ResetDelay());
                }
            }
        }

        else if (resetDelay == false)
        {
            if (delayCoroutine != null) StopCoroutine(delayCoroutine);
            delayCoroutine = StartCoroutine(ResetDelay());
        }

        //Nothing seen or heard, remain in this state
        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        resetDelay = false;
        returnToPreviousBehavior = false; 
        if (delayCoroutine != null) StopCoroutine(delayCoroutine);
    }

    private IEnumerator ResetDelay()
    {
        resetDelay = true;
        yield return new WaitForSeconds(returnToStartTime);
        returnToPreviousBehavior = true;
        resetDelay = false;
    }
}
