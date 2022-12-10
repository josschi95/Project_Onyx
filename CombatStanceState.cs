using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This state works as a mediator between the chase state and the attack state
 Choosing between the two depending on attack cooldown and distance to target*/

public class CombatStanceState : _State
{
    public override _State OnStateEnter(NPCController controller)
    {
        controller.hasEngaged = true;
        controller.StopAgent(true);
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        if (controller.target == null) return controller.OnTargetLost();

        controller.FaceTarget(controller.target.transform.position);
        controller.TrackTarget(); //Continuously update their known position

        //Get a new attack if the current one is null
        if (combat.currentAttack == null) handler.attack.GetNewAttack(controller, combat);

        //Chase target if they are too far
        if (controller.distanceFromTarget > combat.attackRange) return handler.chase;

        //If cooldown is over and close enough to attack, Attack
        if (combat.recoveringFromAttack == false && controller.distanceFromTarget <= combat.attackRange) return handler.attack;

        //Put up shield if they have one
        if (combat.shield != null)
        {
            //maybe separate function for setBlocking
            controller.animHandler.anim.SetBool("blocking", true);
            controller.OnSpeedChange(controller.walkSpeed);
        }
        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        controller.StopAgent(false);
    }
}