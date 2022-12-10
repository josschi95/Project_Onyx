using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : CharacterController
{
    #region - References -
    private NPCStats stats;
    [SerializeField] public NPCCombat npcCombat;
    [SerializeField] public AICharacterDetection charDetect;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public NPCEquipmentManager equipment;
    [HideInInspector] public Vector3 lastTargetPosition = Vector3.zero;
    [HideInInspector] public NPCLog log;
    #endregion

    #region - Character Detection -
    [Header("Character Detection")]
    public CharacterController target;
    public float distanceFromTarget;// {get; private set; }
    public bool isHostile;
    #endregion

    #region - NPC Behavior -
    public float defaultStoppingDistance = 1.5f;
    //The NPC has moved to and attacked the player
    [HideInInspector] public bool hasEngaged = false;
    //The player has attacked the NPC
    [HideInInspector] public bool targetHasEngaged = false;
    [HideInInspector] public bool movingToFlank = false;
    #endregion

    public bool debugging = false;
    public bool targetDummy = false;

    [Header("Behavior States")]
    [SerializeField] protected _StateHandler handler;
    [SerializeField] protected _State currentState;
    public CombatBehavior combatBehavior;

    protected float turning;
    protected float lastRot;

    private bool multiLayerAnim = false;

    protected override void Start()
    {
        base.Start();
        stats = GetComponent<NPCStats>();
        log = GetComponent<NPCLog>();
        if (log == null) gameObject.AddComponent<NPCLog>();
        log = GetComponent<NPCLog>();

        characterStats.onDamageTaken += OnDamageTaken;
        characterStats.onDeath += StopAllCoroutines;
        characterStats.onUnconscious += StopAllCoroutines;

        handler.defaultState.OnStateEnter(this);
        currentState = handler.defaultState;

        animHandler.anim.SetBool("isGrounded", true);
        animHandler.anim.SetBool("isCrouching", false);
        rb.drag = groundDrag;

        if (animHandler.anim.layerCount > 1)
            multiLayerAnim = false;
        else multiLayerAnim = true;
        Debug.Log("multiLayerAnim " + multiLayerAnim);
    }

    protected override void Update()
    {
        //base.Update();
        AnimationsControl();
        //Returns before any behavior is executed
        if (targetDummy == true) return;

        //Manages NPC Behavior
        HandleStateMachine();

        if (transform.rotation.y != lastRot)
        {
            turning = (transform.rotation.y - lastRot) * 100;
        }
        else turning = 0;

        lastRot = transform.rotation.y;
    }

    public override void AnimationsControl()
    {
        animHandler.anim.SetBool("isGrounded", true);
        float speedPercent = agent.velocity.magnitude / sprintSpeed;
        animHandler.anim.SetFloat("speedPercent", speedPercent, locomotionSmoothing, Time.deltaTime);
    }

    #region - State Machine Handling -
    private void HandleStateMachine() //Run in update
    {
        if (currentState != null)
        {
            _State nextState = currentState.Tick(this, stats, npcCombat);
            if (nextState != null) SwitchToNextState(nextState);
        }
        else currentState = handler.defaultState;
    }

    protected void SwitchToNextState(_State state)
    {
        if (currentState != state)
        {
            currentState.OnStateExit(this);

            currentState = state.OnStateEnter(this);
            if (debugging == true) log.AddEntry("current state: " + currentState.name);
        }
        else currentState = state;
    }

    public _State ResetToDefault()
    {
        OnSoundClear();
        hasEngaged = false;
        targetHasEngaged = false;
        distanceFromTarget = 0;
        lastTargetPosition = Vector3.zero;

        //Switch back to primary weapon set
        if (equipmentManager.usingSecondaryWeaponSet)
            equipmentManager.SwapWeaponSets(false, true);

        return handler.defaultState;
    }
    #endregion

    #region - Navmesh Agent -
    public void StopAgent(bool isStopped)
    {
        if (agent.enabled) agent.isStopped = isStopped;
    }

    public void UpdateAgentDestination(Vector3 target)
    {
        //probably include some logic in here for whatever
        agent.SetDestination(target);
    }

    public void OnSpeedChange(float newSpeed)
    {
        agent.speed = newSpeed;
    }

    public bool CanReachTarget(Vector3 target)
    {
        var path = new NavMeshPath();
        if (agent.CalculatePath(target, path) && path.status == NavMeshPathStatus.PathComplete) return true;
        return false;
    }
    #endregion

    #region - Sound Detection -
    public bool soundDetected { get; private set; }
    public Vector3 soundSourcePosition { get; private set; }

    public void OnSoundDetected(Vector3 soundPosition)
    {
        soundDetected = true;
        soundSourcePosition = soundPosition;
    }

    public void OnSoundClear()
    {
        soundDetected = false;
        soundSourcePosition = Vector3.zero;
    }
    #endregion

    #region - Character Tracking and Chasing -
    public bool HasLineOfSight(Vector3 spot)
    {
        Vector3 fromPosition = spot += new Vector3(0, 1.5f, 0);
        Vector3 toPosition = target.center.position;
        Vector3 direction = toPosition - fromPosition;
        Debug.DrawLine(fromPosition, toPosition, Color.black);

        RaycastHit hit;
        Ray ray = new Ray(fromPosition, direction);
        if (Physics.Raycast(ray, out hit, 25, charDetect.detectionLayers) && hit.collider.GetComponent<CharacterController>() == target) return true;

        return false;
    }

    public void FaceTarget(Vector3 location)
    {
        Vector3 direction = (location - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void OnDamageTaken(CharacterStats attacker, float damage, DamageType type, bool isLethal)
    {
        //Use this to determine if a crime has been comitted
        //to have special responses to damage types
        //to determine if an assault is lethal or not

        if (isHostile == false) isHostile = true;

        OnSoundClear();
        if (attacker != null)
        {
            Vector3 location = attacker.transform.position;
            float lastX = location.x;
            float lastY = location.y;
            float lastZ = location.z;
            lastTargetPosition = new Vector3(lastX, lastY, lastZ);
        }

        targetHasEngaged = true;
        if (currentState == handler.defaultState || currentState is InvestigateState)
        {
            SwitchToNextState(handler.search);
        }
    }

    public void TrackTarget()
    {
        if (target == null) return;
        //Constantly update last known location of target
        Vector3 position = target.transform.position;
        lastTargetPosition = position;
        distanceFromTarget = Vector3.Distance(target.transform.position, transform.position);
    }

    public _State OnTargetLost()
    {
        if (combatBehavior == CombatBehavior.Reserved) return handler.defaultState;
        else if (combatBehavior == CombatBehavior.Patient) return handler.defaultState;
        return handler.search;
    }
    #endregion

    #region - Dialogue -
    [HideInInspector] public CharacterController conversant;
    private _State interruptedState;
    public override void OnConversationStart(Transform actor)
    {
        interruptedState = currentState;
        SwitchToNextState(handler.dialogue);
        base.OnConversationStart(actor);
    }

    public override void OnConversationEnd(Transform actor)
    {
        base.OnConversationEnd(actor);
        SwitchToNextState(interruptedState);
        conversant = null;
    }
    #endregion

    #region - Death & KO -
    public override void FallUnconscious()
    {
        SwitchToNextState(handler.death);
        PlayerManager.instance.OnCombatChange(this, false);

        if (multiLayerAnim)
            animHandler.anim.SetLayerWeight(1, 0);
        animHandler.anim.SetTrigger("die"); 
        SetRigidbodyState(false);
        SetColliderState(true);
    }

    public override void OnDeath()
    {
        SwitchToNextState(handler.death);
        PlayerManager.instance.OnCombatChange(this, false);

        if (multiLayerAnim)
            animHandler.anim.SetLayerWeight(1, 0);
        animHandler.anim.SetTrigger("die");
        SetRigidbodyState(false);
        SetColliderState(true);
    }
    #endregion
}