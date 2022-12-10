using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : NPCController
{
    protected override void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        characterCombat = GetComponent<CharacterCombat>();
        equipmentManager = GetComponent<EquipmentManager>();
        inventory = GetComponent<Inventory>();
        characterStats.onDeath += OnDeath;
        characterStats.onUnconscious += FallUnconscious;

        log = GetComponent<NPCLog>();
        if (log == null) gameObject.AddComponent<NPCLog>();
        log = GetComponent<NPCLog>();

        characterStats.onDamageTaken += OnDamageTaken;
        characterStats.onDeath += StopAllCoroutines;
        characterStats.onUnconscious += StopAllCoroutines;

        handler.defaultState.OnStateEnter(this);
        currentState = handler.defaultState;

        rb.drag = groundDrag;
    }

    public override void AnimationsControl()
    {
        float speedPercent = agent.velocity.magnitude / sprintSpeed;
        animHandler.anim.SetFloat("speedPercent", speedPercent, locomotionSmoothing, Time.deltaTime);
        //animHandler.anim.SetFloat("turning", turning);
    }

    public override void OnDeath()
    {
        SwitchToNextState(handler.death);
        PlayerManager.instance.OnCombatChange(this, false);

        animHandler.anim.SetTrigger("die");
        SetRigidbodyState(false);
        SetColliderState(true);
    }
}
