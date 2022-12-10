using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "NPC Actions/Attack Action")]
public class NPCAttackAction : NPCAction
{
    public ActionAnimParam animParam;
    [Tooltip("True for powerAttacks, left Handed Throws")]
    public bool altAction = false;
    public int maxCount = 1;

    public float minRange;
    public float maxRange;
    public bool weaponRange = false;

    public float cooldown;
}

public enum ActionAnimParam { Trigger, Bool, Play }
public enum AttackRange { Melee, Leap, Ranged }
