using System.Collections;
using UnityEngine;

public class IdleState : _State
{
    private Vector3 startPos = Vector3.zero;
    private Quaternion startRot = Quaternion.identity;

    public bool resetPositionOnEnter = false;
    private Coroutine resetPosition;

    public override _State OnStateEnter(NPCController controller)
    {
        if (startPos == Vector3.zero)
        {
            startPos = controller.transform.position;
            startRot = controller.transform.rotation;
        }

        controller.ResetToDefault();
        controller.charDetect.ToggleDetection(controller.isHostile);

        if (resetPositionOnEnter == true)
        {
            if (resetPosition != null) StopCoroutine(resetPosition);
            resetPosition = StartCoroutine(ResetPosition(controller));
        }
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

        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
      if (resetPosition != null) StopCoroutine(resetPosition);
    }

    private IEnumerator ResetPosition(NPCController controller)
    {
        controller.UpdateAgentDestination(startPos);
        while (Vector2.Distance(controller.transform.position, startPos) > 0.1)
        {
            yield return null;
        }

        while (controller.transform.rotation != startRot)
        {
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, startRot, Time.deltaTime * 5f);
            yield return null;
        }
    }
}