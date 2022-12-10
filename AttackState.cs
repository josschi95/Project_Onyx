using System.Collections.Generic;
using UnityEngine;

public class AttackState : _State
{
    private NPCAttackAction lastAttack;

    public override _State OnStateEnter(NPCController controller)
    {
        controller.StopAgent(true);
        controller.lastTargetPosition = Vector3.zero;
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        if (combat.recoveringFromAttack || controller.acceptInput == false) return handler.combatStance;
        else if (controller.target == null) return handler.search;
        //else if (controller.distanceFromTarget > combat.attackRange) return handler.chase;

        controller.FaceTarget(controller.target.transform.position);
        controller.TrackTarget(); //Continuously update their known position

        if (combat.currentAttack == null)
        {
            GetNewAttack(controller, combat);
            return handler.chase;
        }
        else Attack(combat, combat.currentAttack);

        return handler.combatStance;
    }

    public override void OnStateExit(NPCController controller)
    {
        controller.StopAgent(false);
    }

    //Add logic for stamina cost and powerAttacks
    //Add logic for current mana and spells
    //Actually no, that will be a new State, this should be purely physical attacks
    //Add logic for switching between weapon sets and 
    //Run some logic to see if the target CAN be reached e.g. if they're on a ledge or something, automatically default to ranged attacks
    //I don't think I'd run that logic here, that would likely be in Chase State since this state is purely for deciding which attack to make
    //Options at current range
    //Preferred Range
    //Can get to preferred range

    //Advanced options (different state, deal with that later
    public void GetNewAttack(NPCController controller, NPCCombat combat)
    {
        NPCAttackAction nextAttack = null;

        var list = new List<NPCAttackAction>();
        //Make a list of all actions that are available at the current range
        for (int i = 0; i < combat.attackActions.Length; i++)
        {
            if (controller.distanceFromTarget > combat.attackActions[i].maxRange) continue;
            if (controller.distanceFromTarget < combat.attackActions[i].minRange) continue;
            list.Add(combat.attackActions[i]);
        }

        //If there is only one viable option, exit early
        if (list.Count == 0)
        {
            Debug.Log("No valid attacks");
            //Gonna run into some issues here
        }
        else if (list.Count == 1) nextAttack = list[0];
        else
        {
            int i = Random.Range(0, list.Count+1);
            nextAttack = list[i];
        }

        combat.SetNextAttack(nextAttack);
    }

    private void Attack(NPCCombat combat, NPCAttackAction attack)
    {
        lastAttack = attack;
        if (attack.animParam == ActionAnimParam.Trigger)
        {
            if (attack.altAction) combat.PowerAttack(attack.actionAnimationName, attack.cooldown);
            else
            {
                int num = Random.Range(1, attack.maxCount + 1);
                if (num == 1) combat.SingleAttack(attack.actionAnimationName, attack.cooldown);
                else combat.ComboAttack(attack.actionAnimationName, num);
            }
        }
        else if (attack.animParam == ActionAnimParam.Bool)
        {
            combat.RangedAttack(!attack.altAction);
        }
    }
}