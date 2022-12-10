using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueState : _State
{
    public override _State OnStateEnter(NPCController controller)
    {
        controller.ResetToDefault();
        controller.StopAgent(true);
        controller.charDetect.ToggleDetection(controller.isHostile);
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        controller.FaceTarget(controller.conversant.transform.position);
        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        controller.StopAgent(false);
    }
}
