using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntState : _State
{
    private bool onEnterRegistered = false;

    public override _State OnStateEnter(NPCController controller)
    {
        //immediately exit out of this state if the target is too close
        //if (controller.distanceFromTarget < 5) return handler.combatStance;
        if (controller.targetHasEngaged) return handler.combatStance;

        controller.animHandler.anim.Play("taunt");
        onEnterRegistered = true;
        controller.OnSoundClear();
        controller.lastTargetPosition = Vector3.zero;
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        if (controller.target == null) return controller.OnTargetLost();

        controller.FaceTarget(controller.target.transform.position);
        controller.TrackTarget(); //Continuously update their known position
        //controller.distanceFromTarget = Vector3.Distance(controller.target.transform.position, controller.transform.position);
        if (onEnterRegistered == false)
        {
            if (controller.targetHasEngaged) return handler.combatStance;

            controller.animHandler.anim.Play("taunt");
            onEnterRegistered = true;
            return this;
        }

        var info = controller.animHandler.anim.GetCurrentAnimatorClipInfo(0);
        if (info[0].clip != null && info[0].clip.name == controller.animHandler.taunt.name) return this;

        return handler.combatIdle;
    }

    public override void OnStateExit(NPCController controller)
    {
        handler.combatIdle.lastTaunt = Time.time;
        onEnterRegistered = false;
    }
}
