using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <NOTES>
/// What am I using bowDrawn for? Is it just for enemies to see if I'm aiming a bow at them?
/// If that's it then just get rid of it
/// 
/// Will need to make changes to FireProjectile in this script and in NPCCombat once I get around to pooling the prefabs
/// </NOTES>
public class PlayerCombat : CharacterCombat
{
    #region - Singleton -

    public static PlayerCombat instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerCombat found");
            return;
        }

        instance = this;
    }

    #endregion

    private Player_Settings settings;
    private PlayerController playerController;
    [SerializeField] private PlayerPerkManager playerPerks;

    public Transform clavicle_L;
    public Transform clavicle_R;
    private CameraHelper cameraHelper;
    private Camera cam;

    private bool blockRight;
    private bool blockLeft;
    public bool defaultAttack;

    //Used to detect if the player has released the LMB/RMB while the game is paused
    private bool rightDown;
    private bool leftDown;

    public override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        playerPerks = PlayerPerkManager.instance;
        cameraHelper = CameraHelper.instance;
        cam = cameraHelper.mainCam;
        settings = Player_Settings.instance;
        settings.onSettingsChanged += delegate { defaultAttack = settings.useDefaultAttack; };
        GameMaster.instance.onGamePaused += RetainHeldInput;
    }
    
    public override void ReadyWeapons()
    {
        if (GameMaster.instance.gamePaused || playerController.acceptInput == false) return;
        base.ReadyWeapons();
    }

    public override WeaponDamage GetWeaponDamage(Weapon weapon)
    {
        if (weapon.weaponType != WeaponType.Bow && playerPerks.HasPerk(playerPerks.perkManager.turnTheTides))
        {
            if (characterStats.currentHealth <= characterStats.statSheet.maxHealth.GetValue() * 0.5f)
            {
                float multiplier = (1 + (1 - (characterStats.currentHealth / characterStats.statSheet.maxHealth.GetValue())));
                var dmg = base.GetWeaponDamage(weapon);
                return new WeaponDamage(dmg.magnitude * multiplier, dmg.type);
            }
        }
        return base.GetWeaponDamage(weapon);
    }

    public override AttackDirection GetAttackDirection(Weapon weapon)
    {
        if (defaultAttack) return weapon.defaultAttack;
        else if (settings.attackDirection == 1) //Movement based
        {
            if (playerController.verticalMovement == 0 && playerController.horizontalMovement == 0) return weapon.defaultAttack;
            else if (playerController.verticalMovement > 0) return AttackDirection.Thrust; //Thrust when moving forward
            else if (playerController.verticalMovement < 0) return AttackDirection.Bash; //Overhead when moving back
            else if (playerController.horizontalMovement != 0) return AttackDirection.Slash; //Slash when moving sideways
            return weapon.defaultAttack;
        }
        else if (settings.attackDirection == 2)
        {
            if (playerController.cameraLook.x == 0 && playerController.cameraLook.y == 0) return weapon.defaultAttack;
            else if (playerController.cameraLook.y > 0) return AttackDirection.Thrust; //Thrust when sliding up
            else if (playerController.cameraLook.y < 0) return AttackDirection.Bash; //Overhead when sliding down
            else if (playerController.cameraLook.x != 0) return AttackDirection.Slash; //Slash when sliding sideways
            return weapon.defaultAttack;
        }
        return base.GetAttackDirection(weapon);
    }

    public override void FireProjectile(ActiveWeapon activeWeapon)
    {
        GameObject newProjectile = null;
        Weapon spentAmmo = null;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        bool autoReturn = projectileReturn;

        //Instantiating
        if (activeWeapon.weapon is ThrownWeapon thrown)
        {
            if (activeWeapon.runed == true) autoReturn = true;
            newProjectile = Instantiate(thrown.projectile, strikingPoint.position, cam.transform.rotation);

            spentAmmo = thrown;
        }
        else if (activeWeapon.weapon is Arrow arrow)
        {
            if (equipManager.mainHand.runed == true) autoReturn = true;
            GameObject bowArrow = equipManager.mainHand.GetComponentInChildren<BowAnimHelper>().arrow;
            newProjectile = Instantiate(arrow.arrowPrefab, bowArrow.transform.position, cam.transform.rotation);
            spentAmmo = arrow;
        }
        else Debug.LogWarning("Not instantiating an arrow or thrown weapon");
        if (Physics.Raycast(ray, out hit, 100)) newProjectile.transform.LookAt(hit.point);

        Projectile projectileStats = newProjectile.GetComponentInChildren<Projectile>();
        projectileStats.OnSpawn(this, activeWeapon.weapon, activeWeapon.poison, autoReturn);

        PlayerInventory.instance.RemoveItem(spentAmmo, 1);
    }

    private void OnMeleeAttack(bool mainHand, bool powerAttack)
    {
        ClearTargetList();
        if (powerAttack && characterStats.currentStamina > 10) animator.SetTrigger("powerAttack");

        if (equipManager.currentLoadout != WeaponLoadout.Unarmed)
        {
            Weapon weapon = null;

            if (mainHand && equipManager.mainHand != null) weapon = equipManager.mainHand.weapon;
            else if (!mainHand && equipManager.offHand != null) weapon = equipManager.offHand.weapon;
            lastAttackDir = GetAttackDirection(weapon);
            animator.SetInteger("attackDirection", (int)lastAttackDir);
        }
        else
        {
            animator.SetInteger("attackDirection", 0);
            lastAttackDir = AttackDirection.Thrust;
        }

        if (mainHand) animator.SetTrigger("attack1");
        else animator.SetTrigger("attack2");
    }

    private void OnBlock(bool blockRight)
    {
        animator.SetBool("blocking", true);
        OnStartBlock();
        if (blockRight) this.blockRight = true;
        else blockLeft = true;
    }

    private bool CanAttack()
    {
        if (GameMaster.instance.gamePaused || playerController.acceptInput == false) return false;
        if (weaponsDrawn == false) return false;
        return true;
    }

    #region - Right Hand -
    //LMB Press
    public void Press_Primary()
    {
        if (CanAttack() == false) return;
        if (playerController.isSprinting) playerController.OnSprint(false);

        //Quick Throw/Bow
        if (InputHandler.AlternateClick_Static()) Quickthrow_Right();
        else if (equipManager.mainHand == null && equipManager.currentLoadout != WeaponLoadout.Unarmed) OnBlock(true);
        else OnMeleeAttack(true, false);
    }

    //LMB Hold
    public void Hold_Primary()
    {
        rightDown = true;
        if (CanAttack() == false) return;
        //if (animator.GetBool("holdPrimary")) return; //Player hasn't let up on the button since it was set
        if (playerController.isSprinting) playerController.OnSprint(false);

        if (InputHandler.AlternateClick_Static()) Throw_Right();
        else if (equipManager.currentLoadout == WeaponLoadout.Bow) OnDrawBow();
        else if (equipManager.currentLoadout == WeaponLoadout.Unarmed) OnMeleeAttack(true, true);
        else
        {
            if (equipManager.mainHand == null) OnBlock(true);
            else OnMeleeAttack(true, true);
        }
    }

    //LMB Release
    public void Release_Primary()
    {
        rightDown = false;
        if (GameMaster.instance.gamePaused || playerController.acceptInput == false) return;

        characterBowDrawn = false;

        animator.SetBool("holdPrimary", false);
        if (blockRight)
        {
            blockRight = false;
            animator.SetBool("blocking", false);
        }
        if (bowAnim != null)
        {
            bowAnim.SetDrawWeight(0);
            bowAnim.ReleaseArrow();
        }
    }
    #endregion

    #region - Left Hand -
    //RMB Press
    public void Press_Secondary()
    {
        if (CanAttack() == false) return;
        if (playerController.isSprinting) playerController.OnSprint(false);

        //Quick Throw
        if (InputHandler.AlternateClick_Static()) Quickthrow_Left();
        else if (equipManager.offHand == null && equipManager.currentLoadout != WeaponLoadout.Unarmed) OnBlock(false);
        else animator.SetTrigger("attack2");
    }

    //RMB Hold
    public void Hold_Secondary()
    {
        leftDown = true;
        if (CanAttack() == false || hasTwoHander == true) return;
        //if (animator.GetBool("holdSecondary")) return; //Player hasn't let up on the button since it was set
        if (playerController.isSprinting) playerController.OnSprint(false);

        if (InputHandler.AlternateClick_Static()) Throw_Left();
        else if (shield != null || equipManager.mainHand != null) OnBlock(false);
        else OnMeleeAttack(false, true);
    }

    //RMB Release
    public void Release_Secondary()
    {
        leftDown = false;
        if (GameMaster.instance.gamePaused || playerController.acceptInput == false) return;
        animator.SetBool("holdSecondary", false);
        if (blockLeft)
        {
            blockLeft = false;
            animator.SetBool("blocking", false);
        }
    }
    #endregion

    #region - Ranged Weapons -
    private void OnDrawBow()
    {
        if (equipManager.projectilesOff == null)
            if (equipManager.CheckForArrows() == false)
            {
                UIManager.instance.DisplayPopup("No Arrows");
                return;
            }
        characterBowDrawn = true;
        animator.SetBool("holdPrimary", true);
        //StartCoroutine(aimCam.AimZoomIn(1f)); //(0.5f);
        //StartCoroutine(TiltShoulder());
    }

    //LMB Press + Alt
    private void Quickthrow_Right()
    {
        if (equipManager.mainHand != null && equipManager.mainHand.weapon is ThrownWeapon)
        {
            animator.SetTrigger("quickThrow1");
        }
        else UIManager.instance.DisplayPopup("No Throwing Weapons Equipped");
    }

    //LMB Hold + Alt
    private void Throw_Right()
    {
        if (equipManager.mainHand != null && equipManager.mainHand.weapon is ThrownWeapon)
        {
            characterBowDrawn = true;
            animator.SetBool("holdPrimary", true);
        }
        else UIManager.instance.DisplayPopup("No Throwing Weapons Equipped");
    }

    //RMB Press + Alt
    private void Quickthrow_Left()
    {
        if (equipManager.offHand != null && equipManager.offHand.weapon is ThrownWeapon)
        {
            animator.SetTrigger("quickThrow2");
        }
        else UIManager.instance.DisplayPopup("No Throwing Weapons Equipped");
    }

    //RMB Hold + Alt
    private void Throw_Left()
    {
        if (equipManager.offHand != null && equipManager.offHand.weapon is ThrownWeapon)
        {
            characterBowDrawn = true;
            animator.SetBool("holdSecondary", true);
        }
    }
    #endregion

    private void RetainHeldInput()
    {
        bool holdRight = animator.GetBool("holdPrimary");
        bool holdLeft = animator.GetBool("holdSecondary");

        if (holdRight == true || holdLeft == true)
        {
            StartCoroutine(HoldInput(holdRight, holdLeft));
        }
    }

    private IEnumerator HoldInput(bool holdRight, bool holdLeft)
    {
        while (GameMaster.instance.gamePaused)
        {
            yield return null;
        }
        //The player paused the game while holding the LMB, and resumed with it released
        if (holdRight && !rightDown) Release_Primary();
        //Same as above but for RMB
        if (holdLeft && !leftDown) Release_Secondary();
    }

    #region - Aim Up/Down -
    /*private void LateUpdate()
    {
        TiltShoulders();
    }*/

    void TiltShoulders()
    {
        if (hasBow)
        {
            if (cameraHelper.aimCamActive == true)
            {
                float shoulderL = playerController.turn.y - 45;
                clavicle_L.transform.localRotation = Quaternion.Euler(clavicle_L.transform.localEulerAngles.x, clavicle_L.transform.localEulerAngles.y, shoulderL);
            }
        }
    }

    private IEnumerator TiltShoulder()
    {
        if (hasBow)
        {
            while (characterBowDrawn == true || cameraHelper.aimCamActive == true)
            {
                float shoulderL = playerController.turn.y - 45;
                clavicle_L.transform.localRotation = Quaternion.Euler(clavicle_L.transform.localEulerAngles.x, clavicle_L.transform.localEulerAngles.y, shoulderL);
                yield return null;
            }
        }
    }
    #endregion
}