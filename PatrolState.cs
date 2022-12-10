using System.Collections;
using UnityEngine;

public class PatrolState : _State
{
    public float minWaitTime = 20f;
    public float maxWaitTime = 90f;
    public bool waiting = false;

    private int waypointIndex = -1;
    private Transform waypointTarget;
    public Transform[] patrolWaypoints;
    private Coroutine delayCoroutine;

    public override _State OnStateEnter(NPCController controller)
    {
        if (patrolWaypoints.Length == 0 || patrolWaypoints[0] == null)
        {
            Debug.LogWarning("ERROR: No Waypoints assigned");
            handler.defaultState = handler.idle;
            return handler.idle;
        }

        controller.charDetect.ToggleDetection(controller.isHostile);
        //controller.StopAgent(false);
        controller.agent.speed = controller.walkSpeed;
        controller.agent.stoppingDistance = controller.defaultStoppingDistance;
        if (waypointIndex < 0) IterateWaypointIndex();
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

        if (Vector3.Distance(controller.transform.position, waypointTarget.position) < 1.6 && waiting == false)
        {
            IterateWaypointIndex();
            if (delayCoroutine != null) StopCoroutine(delayCoroutine);
            delayCoroutine = StartCoroutine(UpdateDestinationDelay(controller));
        }

        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        waiting = false;
        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);
    }

    //Update to next destination while patrolling
    public void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == patrolWaypoints.Length)
        {
            waypointIndex = 0;
        }
        waypointTarget = patrolWaypoints[waypointIndex];
    }

    IEnumerator UpdateDestinationDelay(NPCController controller)
    {
        waiting = true;
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);
        UpdateDestination(controller);
    }

    public void UpdateDestination(NPCController controller)
    {
        controller.UpdateAgentDestination(waypointTarget.position);
        waiting = false;
    }
}