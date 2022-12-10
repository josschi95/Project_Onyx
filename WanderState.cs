using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : _State
{
    private Vector3 startPos = Vector3.zero;
    private Vector3 wanderPoint;
    private bool isWaiting = false;
    private Coroutine delayCoroutine;

    [Range(10, 100)] public float wanderRadius;
    public bool controlledWander = true;

    public float minWaitTime = 20f;
    public float maxWaitTime = 90f;

    public override _State OnStateEnter(NPCController controller)
    {
        if (startPos == Vector3.zero) startPos = controller.transform.position;
        controller.charDetect.ToggleDetection(controller.isHostile);

        controller.agent.speed = controller.walkSpeed;
        controller.agent.stoppingDistance = controller.defaultStoppingDistance;

        IterateWanderLocation(controller);
        UpdateDestination(controller);
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        if (controller.isHostile)
        {
            //If a target is found, switch to chasing
            if (controller.target != null) return handler.combatIdle;

            //No target is found, but a sound is heard, switch to investigating
            else if (controller.soundDetected == true) return handler.investigate;
        }

        if (controller.agent.remainingDistance <= controller.agent.stoppingDistance && isWaiting == false)
        {
            if (delayCoroutine != null) StopCoroutine(delayCoroutine);
            delayCoroutine = StartCoroutine(UpdateDestinationDelay(controller));
        }
        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        isWaiting = false;
        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);
    }

    //Update to next destination while wandering
    public void IterateWanderLocation(NPCController controller)
    {
        Vector3 randomPosition = Random.insideUnitSphere * wanderRadius;
        if (controlledWander) randomPosition += startPos;
        else randomPosition += controller.transform.position;

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, wanderRadius, 1))
        {
            if (Vector3.Distance(hit.position, controller.transform.position) > 2)
            {
                wanderPoint = hit.position;
            }
        }
    }

    private void UpdateDestination(NPCController controller)
    {
        IterateWanderLocation(controller);
        controller.UpdateAgentDestination(wanderPoint);
    }

    IEnumerator UpdateDestinationDelay(NPCController controller)
    {
        isWaiting = true;
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);
        UpdateDestination(controller);
        isWaiting = false;
    }
}
