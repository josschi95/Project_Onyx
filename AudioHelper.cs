using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHelper : MonoBehaviour
{
    #region - Singleton -
    public static AudioHelper instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of AudioHelper found");
            Destroy(this);
            return;
        }
        else instance = this;
    }
    #endregion

    [Header("Interactions")]
    public AudioClip itemPickup;

    [Header("Bows")]
    public AudioClip takeArrow;
    public AudioClip drawBow;
    public AudioClip fireArrow;
    public AudioClip projectileImpact;

    [Space]
    public AudioClip[] weaponSwing;
    [Space]
    public AudioClip[] slashingAttacks;
    public AudioClip[] bluntAttacks;
    public AudioClip[] pierceAttacks;
    public AudioClip[] unarmedAttacks;
    [Space]
    public AudioClip[] metalShields;
    public AudioClip[] woodShields;

    public AudioClip UnarmedStrike()
    {
        int num = Random.Range(0, unarmedAttacks.Length);
        return unarmedAttacks[num];
    }

    public AudioClip WeaponSwing()
    {
        int num = Random.Range(0, weaponSwing.Length);
        return weaponSwing[num];
    }

    public AudioClip WeaponImpact(DamageType type)
    {
        switch (type)
        {
            case DamageType.Bludgeoning:
                {
                    int num = Random.Range(0, bluntAttacks.Length);
                    return bluntAttacks[num];
                }
            case DamageType.Piercing:
                {
                    int num = Random.Range(0, pierceAttacks.Length);
                    return bluntAttacks[num];
                }
            case DamageType.Slashing:
                {
                    int num = Random.Range(0, slashingAttacks.Length);
                    return bluntAttacks[num];
                }
        }
        return null;
    }

    public AudioClip ShieldStruck(bool metalShield)
    {
        if (metalShield)
        {
            int num = Random.Range(0, metalShields.Length);
            return metalShields[num];
        }
        else
        {
            int num = Random.Range(0, woodShields.Length);
            return woodShields[num];
        }
    }
}

//Another option for these, rather than choosing a random one each time, would be to put them in a queue/stack and draw from there
//Then reshuffle when the queue/stack is empty