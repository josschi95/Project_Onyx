using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class PlayerController : CharacterController
{
    #region - Singleton -
    public static PlayerController instance;
    protected override void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    //Currently only using this to toggle the camera, that's just fine
    public delegate void OnPlayerDialogue(bool isStarting);
    public OnPlayerDialogue onPlayerDialogueCallback;

    #region - References -
    private UIManager UI;
    private CameraHelper cameraHelper;
    private PlayerEquipmentManager playerEquip;
    private Player_Settings settings;
    #endregion

    public Transform head;
    public Transform arm_L;
    public Transform arm_R;

    #region - Variables -
    [Header("Movement")]
    private float moveSpeed;
    private float acceleration = 10f;
    private float movementMultiplier = 10;
    private float airMultiplier = 0.4f;
    [SerializeField] private float jumpforce = 7.5f;

    public bool isSprinting { get; private set; }
    [HideInInspector] public bool isWalking = false;
    private Coroutine sprintCoroutine;

    #region - Camera Look -
    [Header("Turning")]
    public float aimingXTurnSensitivity = 1f;
    public float aimingYTiltSensitivity = 1f;
    public GameObject followTarget;
    public float minY = -55;
    public float maxY = 75;
    private float mouseX;
    private float mouseY;
    [HideInInspector] public Vector2 turn;
    private bool freeLook;
    #endregion

    public float verticalMovement { get; private set; }
    public float horizontalMovement { get; private set; }
    [HideInInspector] public Vector2 movementInput; //Could remove both of these to change over to a value gotten from InputHanderl
    [HideInInspector] public Vector2 cameraLook; //same as above
    private Transform camerMainTransform;
    private Vector3 movementDirection;

    #region - Crouching -
    [Header("Crouching")]
    [SerializeField] CapsuleCollider playerCollider;
    private float colliderCenter = 0.89f;
    private float colliderCrouchCenter = 0.7f;
    private float collider1Height = 1.8f;
    private float collider1CrouchHeight = 1.4f;
    Coroutine crouchChange;
    #endregion

    #region - Light -
    [Header("Player Lighting")]
    [SerializeField] private RenderTexture lightCheckTexture;
    private Coroutine playerLightCoroutine;
    public float playerLightLevel;
    #endregion

    [Header("Step Up")]
    [SerializeField] private GameObject stepRayLower;
    [SerializeField] private GameObject stepRayUpper;

    public Transform climbingPoint;
    private bool isClimbing;
    public LayerMask climbableMask;
    public float wallDistance = 0.4f;
    #endregion

    protected override void Start()
    {
        base.Start();
        UI = UIManager.instance;
        rb = GetComponent<Rigidbody>();
        characterCombat = GetComponent<PlayerCombat>();
        playerEquip = GetComponent<PlayerEquipmentManager>();
        settings = Player_Settings.instance;
        cameraHelper = CameraHelper.instance;
        camerMainTransform = cameraHelper.mainCam.transform;
    }

    protected override void Update()
    {
        base.Update();
        //Climbing(Physics.Raycast(climbingPoint.position, transform.forward, wallDistance, climbableMask));
        if (!acceptInput) return;

        MyInput();
        ControlSpeed();
        ControlDrag();
        if (characterCombat.weaponsDrawn == false && movementInput == Vector2.zero) freeLook = true;
        else freeLook = false;
    }

    private void FixedUpdate()
    {
        if (!acceptInput) return;
        MovePlayer();
        StepClimb();
    }

    //Set animation parameter values
    public override void AnimationsControl()
    {
        base.AnimationsControl();

        animHandler.anim.SetFloat("horizontal", horizontalMovement, locomotionSmoothing, Time.deltaTime);
        animHandler.anim.SetFloat("vertical", verticalMovement, locomotionSmoothing, Time.deltaTime);

        float speedPercent = rb.velocity.magnitude / sprintSpeed;
        animHandler.anim.SetFloat("speedPercent", speedPercent, locomotionSmoothing, Time.deltaTime);

        animHandler.anim.SetBool("climbing", isClimbing);
    }

    private void ControlDrag()
    {
        if (isGrounded || onLadder)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    #region - Movement Controls -
    private void MyInput()
    {
        if (acceptInput == false) return;

        horizontalMovement = movementInput.x;
        verticalMovement = movementInput.y;

        movementDirection = camerMainTransform.forward * verticalMovement + camerMainTransform.right * horizontalMovement;
        if (onLadder == true) movementDirection = new Vector3(0, 0, verticalMovement); //if on ladder, only accept verticalMovement
        if (isLevitating == false) movementDirection.y = 0; //Ignore vertical movement if not levitating

        mouseX = cameraLook.x * Time.deltaTime;
        mouseY = cameraLook.y * Time.deltaTime;

        if (characterCombat.isBlocking || characterCombat.characterBowDrawn || characterCombat.castingSpell)
        {
            mouseX *= 0.5f;
            mouseY *= 0.5f;
        }

        if (cameraHelper.currentCamera == CameraView.First_Person)
        {
            turn.x += mouseX;
            turn.y -= mouseY;
        }
        else
        {
            if (settings.invert_X) turn.x -= mouseX * settings.XTurnSensitivity * 2;
            else turn.x += mouseX * settings.XTurnSensitivity * 2;

            if (settings.invert_Y) turn.y += mouseY * settings.YTiltSensitivity * 2;
            else turn.y -= mouseY * settings.YTiltSensitivity * 2;
        }

        turn.y = Mathf.Clamp(turn.y, minY, maxY);
    }

    public void OnSprint(bool startSprint)
    {
        if (GameMaster.instance.gamePaused) return;
        if (startSprint == true)
        {
            if (characterCombat.castingSpell || !isGrounded || !acceptInput) return;
            isSprinting = true;
            if (isCrouching) OnCrouch(false);

            if (sprintCoroutine != null) StopCoroutine(sprintCoroutine);
            sprintCoroutine = StartCoroutine(Sprinting());
        }
        else isSprinting = false;
    }

    public void OnJump()
    {
        if (!isGrounded || isCrouching || !acceptInput || GameMaster.instance.gamePaused)
            return;

        if (characterStats.CheckStaminaCost(10) == false) return;
        OnSprint(false);
        
        characterStats.SpendStamina(10);
        if (verticalMovement >= 0 && horizontalMovement == 0)
        {
            //Jump up/forward
            animHandler.anim.Play("jump", 0);
            rb.velocity = new Vector3(rb.velocity.x, 2, rb.velocity.z);
            rb.AddForce(transform.up * jumpforce, ForceMode.VelocityChange);
        }
        else //Dodge
        {
            animHandler.anim.Play("dash", 0);
            //rb.AddForce(transform.forward * jumpforce * verticalMovement, ForceMode.VelocityChange);
            //rb.AddForce(transform.right * jumpforce * horizontalMovement, ForceMode.VelocityChange);
        }
        ObjectPooler.SpawnFromPool_Static("jump", transform.position, transform.rotation);
    }

    public void OnCrouch(bool startCrouch)
    {
        if (GameMaster.instance.gamePaused) return;

        if (startCrouch == true)
        {
            if (!acceptInput || !isGrounded) return;
            isCrouching = true;
            if (isSprinting) OnSprint(false);
        }
        else isCrouching = false;

        if (crouchChange != null) StopCoroutine(crouchChange);
        crouchChange = StartCoroutine(ChangeCrouchHeight());

        if (playerLightCoroutine != null) StopCoroutine(playerLightCoroutine);
        if (isCrouching) playerLightCoroutine = StartCoroutine(PlayerLighting());
    }

    private void ControlSpeed()
    {
        if (isSprinting) if (verticalMovement < 0.5 || horizontalMovement != 0) OnSprint(false);

        float newSpeed = 0;
        if (overburdened) newSpeed = burdenedSpeed;
        else if (isCrouching) newSpeed = crouchSpeed;
        else if (isLevitating) newSpeed = walkSpeed;
        else if (isSprinting) newSpeed = sprintSpeed;
        else if (isWalking || characterCombat.characterBowDrawn || characterCombat.isBlocking || characterCombat.castingSpell) newSpeed = walkSpeed;
        else if (!isWalking) newSpeed = runSpeed;
        moveSpeed = Mathf.Lerp(moveSpeed, newSpeed, acceleration * Time.deltaTime);
    }

    private void MovePlayer()
    {
        if (acceptInput == false) return;

        #region - Rotation -
        if (cameraHelper.currentCamera == CameraView.First_Person)
        {
            float rot = cameraHelper.FirstPersonRotation();
            transform.rotation = Quaternion.Euler(0, rot, 0);
        }
        else
        {
            if (!freeLook) transform.rotation = Quaternion.Euler(0, turn.x, 0);
            followTarget.transform.rotation = Quaternion.Euler(turn.y, turn.x, 0);
        }

        if (onLadder) transform.rotation = Quaternion.Euler(0, ladderRotation, 0);
        #endregion

        if (isInteracting == true) return;

        //Can only move up and down
        if (onLadder)
        {
            rb.velocity = new Vector3(0, moveSpeed * verticalMovement, 0);
        }
        //Can move up/down and left/right
        else if (isClimbing)
        {
            //Vector3 dir = transform.forward * verticalMovement + transform.right * horizontalMovement;
            //rb.velocity = new Vector3(moveSpeed * horizontalMovement, moveSpeed * verticalMovement, 0);
            rb.velocity = new Vector3(0, moveSpeed * verticalMovement, 0);
        }
        //Move forward/back and left/right
        else if (canMove == true)
        {
            float multiplier = 1;
            if (isGrounded == false) multiplier = airMultiplier;
            rb.AddForce(movementDirection.normalized * moveSpeed * movementMultiplier * multiplier, ForceMode.Acceleration);
            //rb.velocity = (movementDirection.normalized * moveSpeed * multiplier);
        }
    }

    private void Climbing (bool isClimbing)
    {
        if (isCrouching) return;

        this.isClimbing = isClimbing;
        //The player has started climbing
        if (isClimbing && startClimbing == false)
        {
            if (climbingCoroutine != null) StopCoroutine(climbingCoroutine);
            climbingCoroutine = StartCoroutine(ClimbingCoroutine());
        }
    }

    private Coroutine climbingCoroutine;
    private bool startClimbing;
    private IEnumerator ClimbingCoroutine()
    {
        startClimbing = true;
        rb.useGravity = false;
        if (isGrounded == false) animHandler.anim.Play("ladderLoop");
        while (isClimbing)
        {
            //Player is touching the ground and moves back, stop climbing
            if (isGrounded && verticalMovement < 0)
            {
                rb.AddForce(movementDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            }

            yield return null;
        }

        //Raycast to detect wall is no longer detecting a wall, but there is a wall at player's feet
        if (Physics.Raycast(transform.position, transform.forward, wallDistance, climbableMask))
        {
            //There is nothing at the player's head level (e.g. open space)
            if (!Physics.Raycast(climbingPoint.position, transform.forward, wallDistance))
            {
                playerCollider.isTrigger = true;
                animHandler.anim.Play("ladderTop");
                float t = animHandler.anim.GetCurrentAnimatorStateInfo(0).length;
                rb.AddForce(transform.forward * moveSpeed * movementMultiplier, ForceMode.Acceleration);
                yield return new WaitForSeconds(t);
                playerCollider.isTrigger = false;
            }
        }

        startClimbing = false;
        rb.useGravity = true;
    }

    private void StepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.3f))
        {
            //Debug.Log("Ledge Detected");
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.4f))
            {
                //Debug.Log("Ledge Climbable, adding force");
                rb.AddForce(Vector3.up * 20);
            }
        }
    }
    #endregion

    #region - Ladders -
    [HideInInspector] public bool onLadder = false;
    private float ladderRotation;
    public void ClimbLadder(Ladder go)
    {
        onLadder = true;
        animHandler.anim.SetBool("climbing", true);
        animHandler.anim.Play("ladderLoop");
        rb.useGravity = false;
        ladderRotation = go.rotation;
    }

    public void ExitLadder(Ladder go)
    {
        onLadder = false;
        animHandler.anim.SetBool("climbing", false);
        rb.useGravity = true;
    }

    //I need to make some changes here
    //Just teleport the player to the starting position on start
    public IEnumerator ClimbingLadder(Ladder ladder)
    {
        onLadder = true;
        animHandler.anim.SetBool("ladder", true);
        animHandler.anim.Play("ladderLoop");
        rb.useGravity = false;
        ladderRotation = ladder.rotation;
        ladder.beingClimbed = true;

        while (onLadder == true)
        {
            if (Vector3.Distance(transform.position, ladder.ladderBottom.position) <= 0.5 && verticalMovement == -1)
            {
                onLadder = false;
            }

            if (Vector3.Distance(transform.position, ladder.ladderTop.position) <= 2 && verticalMovement == 1)
            {
                animHandler.anim.SetTrigger("ladderTop");
            }
            yield return null;
        }
        animHandler.anim.ResetTrigger("ladderTop");
        ladder.beingClimbed = false;
        animHandler.anim.SetBool("ladder", false);
        rb.useGravity = true;
    }
    #endregion

    public override void OnDeath()
    {
        characterCombat.enabled = false;
        this.enabled = false;
    }

    private IEnumerator Sprinting()
    {
        float weight = playerEquip.apparelWeight;
        float strength = characterStats.statSheet.strength.GetStatDecim();
        float sprintingStaminaCost = Mathf.Clamp((weight / strength), 0.1f, 10);
        while (isSprinting == true)
        {
            characterStats.SpendStamina(sprintingStaminaCost * Time.deltaTime);
            if (characterStats.currentStamina <= 0) isSprinting = false;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ChangeCrouchHeight()
    {
        Debug.Log("I should clean this up and smooth it");
        isGrounded = true;
        yield return new WaitForSeconds(2f);
        if (isCrouching)
        {          
            playerCollider.height = collider1CrouchHeight;
            playerCollider.center = new Vector3(0f, colliderCrouchCenter, 0f);
            isGrounded = true;
        }
        else //Standing from crouch
        {            
            playerCollider.height = collider1Height;
            playerCollider.center = new Vector3(0f, colliderCenter, 0f);
            isGrounded = true;
        }
    }

    #region - Interactions -

    public override IEnumerator Sitting(Chair chair)
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
            if (movementInput != Vector2.zero) isSitting = false;
            yield return null;
        }

        animHandler.anim.SetBool("sitting", false);
        transform.position = chair.departurePosition.position;
        transform.rotation = chair.departurePosition.localRotation;
        chair.ClearOccupant();
    }

    public override IEnumerator Crafting(CraftingStation station, CraftingType type)
    {
        UI.DisplayCraftingScreen(station);
        return base.Crafting(station, type);
    }

    public override IEnumerator Sleeping(Bed bed)
    {
        DateTimeManager.instance.SkipToNextDay();
        return base.Sleeping(bed);
    }
    #endregion


    #region - Camera Adjustment -
    public void ReadjustCameraAngle()
    {
        float camTransitionTime = 0.5f;
        if (turn.y > 0)
        {
            StartCoroutine(ReadjustCameraDown(camTransitionTime));
        }
    }

    private IEnumerator ReadjustCameraDown(float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            turn.y = Mathf.Lerp(turn.y, 0, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region - Dialogue -
    //Don't change these function names, they're callbacks through the DialogueManager system
    public override void OnConversationStart(Transform actor)
    {
        base.OnConversationStart(actor);
        if (onPlayerDialogueCallback != null) 
            onPlayerDialogueCallback.Invoke(true);
    }

    public override void OnConversationEnd(Transform actor)
    {
        base.OnConversationEnd(actor);
        UI.OnDialogueEnd();
        if (onPlayerDialogueCallback != null)
            onPlayerDialogueCallback.Invoke(false);
    }
    #endregion

    #region - Player Lighting -
    private IEnumerator PlayerLighting()
    {
        while (isCrouching)
        {
            playerLightLevel = CalculatePlayerLightLevel();
            yield return null;
        }
    }

    private float CalculatePlayerLightLevel()
    {
        //Main issue here is I'm getting values between 2000 and 6000, so I need to find a minimum value, average value, and max value that I want to go with and apply some sort of multiplier to get the end number to a desired value.
        RenderTexture tempTexture = RenderTexture.GetTemporary(lightCheckTexture.width, lightCheckTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(lightCheckTexture, tempTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tempTexture;

        Texture2D temp2DTexture = new Texture2D(lightCheckTexture.width, lightCheckTexture.height);
        temp2DTexture.ReadPixels(new Rect(0, 0, tempTexture.width, tempTexture.height), 0, 0);
        temp2DTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tempTexture);

        Color32[] colors = temp2DTexture.GetPixels32();
        Destroy(temp2DTexture);

        //returns the light level for the middle of the player, 3/4 of the way up the image, about the middle of their chest
        float intensity = (0.2126f * colors[360300].r) + (0.7152f * colors[360300].g) + (0.0722f * colors[360300].b);

        //This returns the light level for the entire front of the player
        /*
        float intensity = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            intensity += (0.2126f * colors[i].r) + (0.7152f * colors[i].g) + (0.0722f * colors[i].b);
        }
        intensity = Mathf.Round(intensity * 0.00001f);
        */
        
        Debug.Log(intensity);
        return intensity;
    }
    #endregion
}