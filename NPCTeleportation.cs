using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTeleportation : MonoBehaviour
{
    public Transform[] waypoints;
    public bool canTeleport;
    public float teleportCooldown;
    [SerializeField] ParticleSystem teleportFX;
    Coroutine cooldownCoroutine;

    private void Start()
    {
        cooldownCoroutine = StartCoroutine(TeleportCooldown());
    }

    public void Teleport(Transform waypoint)
    {
        Instantiate(teleportFX, transform.position, transform.rotation);
        Instantiate(teleportFX, waypoint.position, waypoint.rotation);
        transform.position = waypoint.position;

        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = StartCoroutine(TeleportCooldown());
    }

    private void FurthestWaypoint()
    {
        float furthestDist = 0; 
        int location = 0;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(PlayerManager.instance.player.transform.position, waypoints[i].position);
            if (dist > furthestDist)
            {
                furthestDist = dist;
                location = i;
            }
        }
        Teleport(waypoints[location]);
    }

    private IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(teleportCooldown);
        if (canTeleport)
        {
            FurthestWaypoint();
        }
        else
        {
            while (canTeleport == false)
            {
                yield return null;
            }
            FurthestWaypoint();
        }
    }
}
