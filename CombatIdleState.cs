using UnityEngine;

 /* A transitory state to handle transitions between the NPC spotting a target and engaging with the target */

public class CombatIdleState : _State
{
    public float patience = 10f;
    public float lastTaunt = -10f;

    public override _State OnStateEnter(NPCController controller)
    {
        controller.StopAgent(true);
        controller.OnSoundClear();
        controller.lastTargetPosition = Vector3.zero;
        if (controller.characterCombat.weaponsDrawn == false)
            controller.characterCombat.ReadyWeapons();

        if (controller.combatBehavior == CombatBehavior.Charge)
        {
            return handler.chase;
        }
        else if (Time.time > lastTaunt + patience)
        {
            return handler.taunt;
        }
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        if (controller.target == null) return controller.OnTargetLost();
        if (controller.targetHasEngaged) return handler.combatStance;

        //Face the target
        controller.FaceTarget(controller.target.transform.position);
        controller.TrackTarget(); //Continuously update their known position

        //If the NPC is Impatient or Patient, they will switch to combatStance if the target moves within their attack range
        if (controller.combatBehavior != CombatBehavior.Reserved && controller.distanceFromTarget < combat.attackRange) return handler.combatStance;

        //Else if they are Impatient and the time since completing their initial taunt is greater than their patience, they'll switch to chasing
        else if (controller.combatBehavior == CombatBehavior.Impatient && Time.time > lastTaunt + patience) return handler.chase;

        //Repeat the taunt if the player still hasn't engaged
        if (Time.time > lastTaunt + patience) return handler.taunt;

        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        controller.StopAgent(false);
    }
}
