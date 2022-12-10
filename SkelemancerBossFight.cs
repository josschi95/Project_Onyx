using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Order of battle
 * player enters the room and triggers the start of the battle
 * after 20? seconds the skelemancer's shield goes down
 * once the skelemancer's HP has been reduced to point1, they teleport back, skeles are summoned, shield is on, crystal shield down
 * crystal 1 is destroyed, skelemancer shield goes down
 * repeat last 3
*/
public class SkelemancerBossFight : MonoBehaviour
{
    public Transform throne;
    [SerializeField] Door[] doors;
    [SerializeField] NPCStats stats;
    [SerializeField] NPCController controller;
    [SerializeField] BossEventHandler bossEvents;
    [SerializeField] NPCTeleportation teleportation;
    [SerializeField] AICharacterDetection detection;
    public float firstShieldDown = 15f;
    [Space]
    [SerializeField] GameObject skelemancerShield;
    [SerializeField] GameObject crystalOneShield;
    [SerializeField] GameObject crystalTwoShield;
    [Space]
    [SerializeField] GameObject[] phaseOneSummons;
    [SerializeField] GameObject[] phaseTwoSummons;


    [SerializeField] Collider[] fightTriggers;
    private bool fightInProgress;

    private void Start()
    {
        teleportation.canTeleport = false;
        teleportation.teleportCooldown = 15f;
        bossEvents.onNextEventCallback += NextPhase;
        stats.onDeath += OnFightEnd;
        StartCoroutine(Immobilize());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fightInProgress == false)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                //Player has entered the final chamber
                fightInProgress = true;
                OnFightStart();
                for (int i = 0; i < fightTriggers.Length; i++)
                {
                    fightTriggers[i].enabled = false;
                }
            }
        }
    }

    public void OnFightStart()
    {
        detection.detectionRadius = 35f;
        doors[0].CloseDoor();
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].enabled = false;
        }
        bossEvents.OnBattleStart();
        StartCoroutine(StartDelay());
        //boss can't move
    }

    private void NextPhase(int phase)
    {
        if (phase == 1)
        {
            PhaseOne();
        }
        else if (phase == 2)
        {
            PhaseTwo();
        }
    }

    private void PhaseOne()
    {
        Debug.Log("Will likely come into some issues here");
        teleportation.Teleport(throne);
        teleportation.teleportCooldown = 10f;
        skelemancerShield.SetActive(true);
        crystalOneShield.SetActive(false);
        StartCoroutine(Immobilize());
        for (int i = 0; i < phaseOneSummons.Length; i++)
        {
            phaseOneSummons[i].SetActive(true);
        }
    }

    private void PhaseTwo()
    {
        teleportation.Teleport(throne);
        teleportation.teleportCooldown = 5f;
        skelemancerShield.SetActive(true);
        crystalTwoShield.SetActive(false);
        StartCoroutine(Immobilize());
        for (int i = 0; i < phaseTwoSummons.Length; i++)
        {
            phaseTwoSummons[i].SetActive(true);
        }
    }

    private void OnFightEnd()
    {
        doors[0].OpenDoor();
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].enabled = true;
        }
        //Gotta have some sort of cool effects here
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(firstShieldDown);
        skelemancerShield.SetActive(false);
        teleportation.canTeleport = true;
    }

    IEnumerator Immobilize()
    {
        while (skelemancerShield.activeSelf == true)
        {
            controller.agent.isStopped = true;
            teleportation.canTeleport = false;
            yield return null;
        }
        teleportation.canTeleport = true;
        controller.agent.isStopped = false;
    }
}
