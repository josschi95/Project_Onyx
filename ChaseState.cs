using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : _State
{
    public float timeToChaseUnseen = 5f;
    private bool chasingGhosts;
    private bool targetLost = false;
    private Coroutine ghostCoroutine;

    public override _State OnStateEnter(NPCController controller)
    {
        targetLost = false;
        controller.StopAgent(false);
        controller.agent.stoppingDistance = controller.npcCombat.attackRange / 2; //Could simplify this
        controller.OnSoundClear();
        chasingGhosts = false;

        //Close the gap between target and NPC
        if (controller.hasEngaged == false && controller.distanceFromTarget > controller.npcCombat.attackRange * 2)
            controller.agent.speed = controller.sprintSpeed;
        
        else controller.agent.speed = controller.runSpeed;
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        //There is no target, switch to searching
        if (controller.target == null || targetLost == true) return handler.search;

        //Face the target
        controller.FaceTarget(controller.target.transform.position);
        controller.TrackTarget(); //Continuously update their known position

        //target is not null but they are outside the character's detection radius
        if (controller.charDetect.detectedCharacters.Contains(controller.target) == false)
        {
            if (chasingGhosts == true) return this;
            else
            {
                if (ghostCoroutine != null) StopCoroutine(ghostCoroutine);
                ghostCoroutine = StartCoroutine(ChaseGhosts(controller));
            }
        }

        //If the character does not currently have an attack chosen, select a new attack
        if (combat.currentAttack == null) handler.attack.GetNewAttack(controller, combat);

        //Within maximum attack range
        if (controller.distanceFromTarget <= combat.attackRange)
        {
            if (combat.hasRanged )
            {
                if (controller.HasLineOfSight(controller.transform.position)) return handler.combatStance;
                else return handler.lineOfSight;

            }
            else return handler.combatStance;
        }

        //Outside Maximum attack range, need to get closer
        controller.UpdateAgentDestination(controller.target.transform.position);

        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        chasingGhosts = false;
        targetLost = false;
        if (ghostCoroutine != null) StopCoroutine(ghostCoroutine);
    }

    //Extends the amount of time that the enemy will chase a target after losing sight
    public IEnumerator ChaseGhosts(NPCController controller)
    {
        chasingGhosts = true;
        controller.log.AddEntry("chasing ghosts");
        yield return new WaitForSeconds(timeToChaseUnseen);
        if (controller.target != null)
        {
            chasingGhosts = false;
            yield break;
        }

        controller.lastTargetPosition = Vector3.zero;
        controller.log.AddEntry("target lost");
        targetLost = true;
    }
}
