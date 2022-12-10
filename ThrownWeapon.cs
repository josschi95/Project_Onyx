using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Thrown Weapon", menuName = "Inventory/Weapon/Throwing")]
public class ThrownWeapon : Weapon
{
    [Header("Thrown Weapon Properties")]
    public GameObject projectile;
    public bool rotate = false;
    //public AnimationClip readyLeft;
    //public AnimationClip throwLeft;
    //public AnimationClip readyRight;
    //public AnimationClip throwRight;
}
