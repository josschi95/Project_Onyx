using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveQuiver : ActiveWeapon
{
    protected override void Start()
    {
        equipSettings = GetComponentInParent<WeaponEquipSettings>();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //Do Nothing
    }

    public override void SwingEffect()
    {
        //Do Nothing
    }
}
