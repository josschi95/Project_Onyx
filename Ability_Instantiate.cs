using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instantiate Ability", menuName = "Ability/Instantiate Ability")]
public class Ability_Instantiate : Ability
{
    [SerializeField] private GameObject objectToSummon;
    public bool instanceFollowsUser;

    public override void Use(CharacterStats stats)
    {
        base.Use(stats);

        Vector3 newPos = stats.transform.position;
        Transform point = null;
        if (instanceFollowsUser == true) point = stats.transform;
        GameObject go = Instantiate(objectToSummon, newPos, Quaternion.identity, point);

        var destroy = go.GetComponent<DestroyOnDelay>();
        if (destroy != null) destroy.duration = duration;
    }
}
