using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LineOfSightState : _State
{
    private float searchRadius;
    private bool movingToFlank = false;

    public override _State OnStateEnter(NPCController controller)
    {
        searchRadius = 1;
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        //There is no target, switch to searching
        if (controller.target == null) return handler.search;

        //Face the target
        controller.FaceTarget(controller.target.transform.position);
        controller.TrackTarget(); //Continuously update their known position

        if (controller.HasLineOfSight(controller.transform.position)) return handler.combatStance;
        
        if (movingToFlank == false)
        {
            Vector3 newPosition = Random.insideUnitSphere * searchRadius;
            newPosition += controller.transform.position;

            if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, searchRadius, 1) && controller.HasLineOfSight(hit.position) == true)
            {
                controller.UpdateAgentDestination(hit.position);
                controller.agent.stoppingDistance = 0.1f;
                StartCoroutine(MovingToFlank(controller, hit.position));
            }
            else searchRadius++;
        }
        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        //
    }

    private IEnumerator MovingToFlank(NPCController controller, Vector3 newPos)
    {
        movingToFlank = true;
        while (Vector3.Distance(controller.transform.position, newPos) > (controller.agent.stoppingDistance + 0.2f) && controller.target != null)
        {
            yield return null;
        }
        movingToFlank = false;
    }
}


