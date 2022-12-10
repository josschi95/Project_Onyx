using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _StateHandler : MonoBehaviour
{
    [Header("Default Behaviors")]
    public _State defaultState;
    [Space]
    public IdleState idle;
    public WanderState wander;
    public PatrolState patrol;

    [Space]
    public DialogueState dialogue;
    public ChaseState chase;
    public InvestigateState investigate;
    public SearchState search;

    [Header("Combat Behaviors")]
    public AttackState attack;
    public CombatIdleState combatIdle;
    public CombatStanceState combatStance;
    public LineOfSightState lineOfSight;
    public TauntState taunt;

    [Space]
    public DeathState death;
}