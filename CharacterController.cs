using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public delegate void OnInteractInterrupt();
    public OnInteractInterrupt onInterruptCallback;

    [HideInInspector] public Inventory inventory;
    [HideInInspector] public CharacterStats characterStats;
    [HideInInspector] public CharacterCombat characterCombat;
    [HideInInspector] public EquipmentManager equipmentManager;
    [HideInInspector] public Rigidbody rb;
    public CharacterAnimator animHandler;
    public Team team;

    public Transform center;

    [Header("Movement Speed")]
    public float burdenedSpeed = 1f;
    public float crouchSpeed = 2f;
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float sprintSpeed = 8f;

    [HideInInspector] public const float locomotionSmoothing = .1f;
    [HideInInspector] public bool overburdened = false;
    [HideInInspector] public bool isCrouching = false;
    //[HideInInspector] 
    public bool acceptInput = true;
    //[HideInInspector] 
    public bool canMove = true;
    [HideInInspector] public bool canRotate = true;

    [HideInInspector] public bool isLevitating = false;
    protected Coroutine levitationCoroutine;

    //[HideInInspector]
    public bool isInteracting = false;
    //[HideInInspector] 
    public bool isSitting = false;
    [HideInInspector] public bool isSleeping = false;

    [Header("Ground Detection")]
    [SerializeField] protected LayerMask groundMask;
    protected float groundDistance = 0.3f;

    [HideInInspector] public float groundDrag = 6f;
    [HideInInspector] public float airDrag = 2f;
    [HideInInspector] public bool isGrounded;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetRigidbodyState(true);
        SetColliderState(false);
    }

    protected virtual void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        characterCombat = GetComponent<CharacterCombat>();
        equipmentManager = GetComponent<EquipmentManager>();
        inventory = GetComponent<Inventory>();
        characterStats.onDeath += OnDeath;
        characterStats.onUnconscious += FallUnconscious;
    }

    protected virtual void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);
        AnimationsControl();
    }

    //Set animation parameter values
    public virtual void AnimationsControl()
    {
        animHandler.anim.SetBool("isGrounded", isGrounded);
        animHandler.anim.SetBool("isCrouching", isCrouching);
    }

    public virtual IEnumerator Sleeping(Bed bed)
    {
        if (characterCombat.weaponsDrawn == true)
            animHandler.anim.SetTrigger("sheathe");

        isSleeping = true;
        //Do some animation shit here

        while (isSleeping == true)
        {
            yield return null;
        }

        //Do some more animation stuff
        bed.ClearOccupant();
    }

    public virtual IEnumerator Sitting(Chair chair)
    {
        if (characterCombat.weaponsDrawn == true)
        {
            animHandler.anim.SetTrigger("sheathe");
        }

        isSitting = true;
        transform.position = chair.seatedPosition.position;
        transform.rotation = chair.seatedPosition.rotation;
        animHandler.anim.SetBool("sitting", true);

        while (isSitting == true)
        {
            transform.rotation = chair.seatedPosition.rotation;
            yield return null;
        }

        animHandler.anim.SetBool("sitting", false);
        transform.position = chair.departurePosition.position;
        transform.rotation = chair.departurePosition.localRotation;
        chair.ClearOccupant();
    }

    public virtual IEnumerator Crafting(CraftingStation station, CraftingType type)
    {
        if (characterCombat.weaponsDrawn == false)
        {
            animHandler.anim.SetTrigger("sheathe");
        }

        isInteracting = true;
        transform.position = station.craftingPosition.position;
        transform.rotation = station.craftingPosition.rotation;
        animHandler.anim.SetBool(type.ToString(), true);

        while (isInteracting == true)
        {
            yield return null;
        }
        animHandler.anim.SetBool(type.ToString(), false);
        station.ClearOccupant();
    }
    
    public virtual void OnLevitateStart(float duraiton)
    {
        if (levitationCoroutine != null) StopCoroutine(levitationCoroutine);
        levitationCoroutine = StartCoroutine(OnLevitate(duraiton));
    }

    public virtual IEnumerator OnLevitate(float duration)
    {
        isLevitating = true;
        rb.useGravity = false;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            isGrounded = true;
            rb.drag = groundDrag;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rb.useGravity = true;
        isLevitating = false;
    }

    #region - Dialogue -
    //Don't change these function names, they're callbacks through the DialogueManager system
    public virtual void OnConversationStart(Transform actor)
    {
        acceptInput = false;
        isInteracting = true;
    }

    public virtual void OnConversationEnd(Transform actor)
    {
        //This gets called when dialogue is exited
        acceptInput = true;
        isInteracting = false;
    }
    #endregion

    #region - Death & Unconsciousness - 
    public virtual void OnDeath()
    {
        animHandler.anim.SetTrigger("die");

        SetRigidbodyState(false);
        SetColliderState(true);
    }

    public virtual void FallUnconscious()
    {
        animHandler.anim.SetTrigger("die");

        SetRigidbodyState(false);
        SetColliderState(true);
    }
    #endregion

    #region - Ragdoll -
    protected virtual void SetRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        rb.isKinematic = !state;
    }

    protected virtual void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }
    #endregion
}