using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterDetection : MonoBehaviour
{
    private NPCController controller;
    private PlayerStats playerStats;

    [SerializeField] private AutoDetection autoDetect;
    [SerializeField] private Transform eyes;
    public SphereCollider detectionArea;
    public CharacterController currentTarget;
    public LayerMask detectionLayers;
    [Space]
    public List<CharacterController> detectableCharacters = new List<CharacterController>();
    public List<CharacterController> detectedCharacters = new List<CharacterController>();
    [Space]
    public float detectionRadius = 25f;
    public float maximumDetectionRadius { get; private set; }

    [Range(40, 90)] [SerializeField] float fieldOfView = 60f;
    [Range(5, 95)] public float perception = 60f;
    public bool canSeeInDarkLight = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (eyes != null)
            Gizmos.DrawWireSphere(eyes.position, detectionRadius);
    }

    private void Start()
    {
        if (detectionArea == null)
        {
            Debug.LogWarning("Need to assign detection area");
            detectionArea = GetComponent<SphereCollider>();
        }

        controller = GetComponent<NPCController>();
        playerStats = PlayerStats.instance;

        maximumDetectionRadius = detectionRadius;
        detectionArea.radius = maximumDetectionRadius;
    }

    private void Update()
    {
        CharacterDetection();
    }

    public void ToggleDetection(bool enabled)
    {
        detectionArea.enabled = enabled;
        autoDetect.ToggleDetection(enabled);
        //controller.log.AddEntry("detection area enabled: " + enabled);
    }

    public void OnDetectionRadiusChange(float range)
    {
        detectionRadius = range;
        detectionArea.radius = range;
        Debug.Log(detectionArea.radius);
    }

    //Determine if a character is Detected by the AI
    private void CharacterDetection()
    {
        if (detectableCharacters.Count == 0) return;

        foreach (CharacterController character in detectableCharacters)
        {
            //Character is dead, Remove them from the list, update Target
            if (character.characterStats.isDead)
            {
                detectableCharacters.Remove(character);
                if (detectedCharacters.Contains(character))
                    detectedCharacters.Remove(character);
                if (currentTarget == character) OnTargetChange(null);
                return;
            }

            //Character is out of range Remove them from the list, update Target
            if (Vector3.Distance(transform.position, character.transform.position) >= detectionRadius)
            {
                detectableCharacters.Remove(character);
                if (detectedCharacters.Contains(character))
                    detectedCharacters.Remove(character);
                if (currentTarget == character) OnTargetChange(null);
                return;
            }

            //Character is within the autoDetect area
            if (autoDetect.autoDetects.Contains(character))
            {
                if (!detectedCharacters.Contains(character))
                    detectedCharacters.Add(character);
                if (controller.debugging)
                    Debug.DrawLine(eyes.position, character.center.position, Color.green);

                if (currentTarget == null)
                {
                    OnTargetChange(character);
                    if (controller.debugging)
                        Debug.DrawLine(eyes.position, currentTarget.transform.position, Color.blue);
                }
            }

            //Character is player and player is sneaking
            else if (character is PlayerController player && player.isCrouching)
            {
                Vector3 charDir = (player.center.position - eyes.position);
                float angle = Vector3.Angle(charDir, eyes.forward);
                if (angle <= fieldOfView)
                {
                    if (PlayerDetected(player)) //I should add a 1 second delay here
                    {
                        if (!detectedCharacters.Contains(player))
                            detectedCharacters.Add(player);

                        if (currentTarget == null) OnTargetChange(player);
                    }
                }
            }

            else
            {
                {
                    Vector3 charDir = (character.center.position - eyes.position);
                    float angle = Vector3.Angle(charDir, transform.forward);

                    if (controller.debugging)
                    {
                        Debug.DrawRay(eyes.position, transform.forward, Color.black);
                        //Debug.Log(angle);
                    }

                    RaycastHit hit;
                    Ray ray = new Ray(eyes.position, charDir);

                    if (Physics.Raycast(ray, out hit, detectionArea.radius, detectionLayers))
                    {
                        CharacterController detectedChar = hit.collider.GetComponentInParent<CharacterController>();
                        if (detectedChar != null && angle <= fieldOfView)
                        {
                            if (!detectedCharacters.Contains(character))
                                detectedCharacters.Add(character);
                            if (controller.debugging)
                                Debug.DrawLine(eyes.position, character.center.position, Color.green);

                            if (currentTarget == null)
                            {
                                OnTargetChange(character);
                                if (controller.debugging)
                                    Debug.DrawLine(eyes.position, currentTarget.transform.position, Color.blue);
                            }
                        }
                        else
                        {
                            if (detectedCharacters.Contains(character))
                                detectedCharacters.Remove(character);
                        }
                    }
                    else
                    {
                        if (detectedCharacters.Contains(character))
                            detectedCharacters.Remove(character);
                    }
                }
            }
        }
    }

    //Add or remove characters from list of Detectable Characters
    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            CharacterController character = other.GetComponent<CharacterController>();
            if (character != null)
            {
                if (character.characterStats.isDead)
                {
                    OnDeadBodyFound(character);
                    return;
                }

                if (character.team == controller.team)
                {
                    //Friendly detected
                }
                else if (controller.team.alliedTeams.Contains(character.team))
                {
                    //Friendly detected
                }
                else
                {
                    if (!detectableCharacters.Contains(character))
                        detectableCharacters.Add(character);
                }
            }
        }
    }

    public void OnTargetChange(CharacterController newTarget)
    {
        if (currentTarget != null) currentTarget.characterStats.onDeath -= OnTargetDeath;

        //detargeting player
        if (currentTarget is PlayerController)
            PlayerManager.instance.OnCombatChange(controller, false);

        currentTarget = newTarget;
        controller.target = newTarget; //I could just make this a callback event... eh

        if (newTarget != null) newTarget.characterStats.onDeath += OnTargetDeath;
        if (newTarget is PlayerController)
            PlayerManager.instance.OnCombatChange(controller, true);

        if (currentTarget == null) FindNearestTarget();
    }

    private void FindNearestTarget()
    {
        if (detectedCharacters.Count == 0)
        {
            controller.lastTargetPosition = Vector3.zero;
            return;
        }

        float num = maximumDetectionRadius;
        int newTarget = 0;
        for (int i = 0; i < detectedCharacters.Count; i++)
        {
            float dist = Vector3.Distance(detectedCharacters[i].transform.position, transform.position);
            if (dist < num)
            {
                num = dist;
                newTarget = i;
            }
        }
        OnTargetChange(detectedCharacters[newTarget]);
    }

    private void OnTargetDeath()
    {
        if (currentTarget.characterStats.isDead)
            OnTargetChange(null);
    }

    private void OnDeadBodyFound(CharacterController corpse)
    {
        //Note: I think I'm disabling the characterController on death, so I'm going to need to find another way to handle this
        if (detectableCharacters.Contains(corpse)) detectableCharacters.Remove(corpse);
        if (corpse.team == controller.team || controller.team.alliedTeams.Contains(corpse.team))
        {
            Debug.Log(transform.name + " found corpse of " + corpse.transform.name);
            //Allied Dead body found, sent out message to other nearby allies
        }
    }

    #region - Player Detection -
    public float PlayerLighting(PlayerController player)
    {
        //Range needs to clamp between 0 and 50
        if (canSeeInDarkLight) return 50f;
        else
        {
            //I'm clamping this for now but it's almost always going to be near 50 with the current settings
            float light = player.playerLightLevel;
            light = Mathf.Clamp(light, 0, 50);
            return light;
        }
    }

    public bool PlayerDetected(PlayerController player)
    {
        float playerVisibility = PlayerVisibility(player);
        if (playerVisibility == 0) return false;

        float playerLighting = PlayerLighting(player);
        float playerSneak = playerStats.statSheet.stealth.GetValue();
        float dist = Vector3.Distance(player.transform.position, transform.position);

        float scoreTotal = 50 + playerSneak - playerVisibility;
        if (scoreTotal <= 0) scoreTotal = 0;
        scoreTotal -= playerLighting;
        if (scoreTotal <= 0) scoreTotal = 0;
        scoreTotal += dist;

        if (scoreTotal <= perception) return true;
        return false;
    }

    #region - Player Visibility -
    public float PlayerVisibility (PlayerController player)
    {
        float playervisibility = 0;
        if (CanSeeBodyPart(player.head.position) == true) playervisibility += 18.75f;
        if (CanSeeBodyPart(player.center.position) == true) playervisibility += 37.5f;
        if (CanSeeBodyPart(player.arm_L.position) == true) playervisibility += 9.375f;
        if (CanSeeBodyPart(player.arm_R.position) == true) playervisibility += 9.375f;
        return playervisibility;
    }

    public bool CanSeeBodyPart(Vector3 spot)
    {
        Vector3 charDir = (spot - eyes.position);
        float angle = Vector3.Angle(charDir, eyes.forward);

        RaycastHit hit;
        Ray ray = new Ray(eyes.position, charDir);

        if (Physics.Raycast(ray, out hit, detectionArea.radius, detectionLayers))
        {
            if (hit.collider.GetComponent<PlayerController>())
            {
                if (controller.debugging)
                    Debug.DrawLine(eyes.position, spot, Color.green);
                return true;
            }
            else
            {
                if (controller.debugging)
                    Debug.DrawLine(eyes.position, spot, Color.red);
            }
        }
        return false;

    }
    #endregion

    //private bool delaying = false;
    private IEnumerator PlayerDetectionDelay()
    {
        //delaying = true;
        yield return new WaitForSeconds(1);
        //delaying = false;
    }

    #endregion
}

/*
private void OnTriggerExit(Collider other)
{
    Debug.Log("On Exit");
    //A character moves beyond the NPCs maximum sight distance
    CharacterController character = other.GetComponent<CharacterController>();
    if (character != null)
    {
        Debug.Log("Not Null");
        if (detectableCharacters.Contains(character))
        {
            Debug.Log("detectable contains");
            detectableCharacters.Remove(character);
        }
        if (detectedCharacters.Contains(character))
        {
            Debug.Log("detected contains");
            detectedCharacters.Remove(character);
        }

        if (currentTarget == character) OnTargetChange(null);
    }
}
*/