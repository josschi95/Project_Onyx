using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shoutout to Kevin Iglesias for this script
public class IKSwitchHelper : MonoBehaviour
{
    private EquipmentManager equipManager;

    public IKHelperHand hand;
    public IKSetting iKSetting;
    public Transform heavyIKSwitch;
    public Transform heavyHandEffector;
    [Space]
    public Transform poleIKSwitch;
    public Transform poleHandEffector;

    private Animator anim;
    private float weight;
    private Coroutine iKCoroutine;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        weight = 0f;
    }

    private void Start()
    {
        equipManager = GetComponentInParent<EquipmentManager>();
        equipManager.onEquipmentChanged += delegate { SwitchActiveTool(); };

        var rightHand = equipManager.mainHand;
        if (rightHand != null)
        {
            if (rightHand.weapon.weaponType == WeaponType.Heavy) iKSetting = IKSetting.Heavy;
            else if (rightHand.weapon.weaponType == WeaponType.Pole) iKSetting = IKSetting.Pole;
            else iKSetting = IKSetting.None;
        }
        else iKSetting = IKSetting.None;
        
        if (iKCoroutine != null) StopCoroutine(iKCoroutine);
        iKCoroutine = StartCoroutine(SetIKWeight());
    }

    /*
    private void Update()
    {
        if (iKSetting != IKSetting.None)
        {
            Transform iKSwitch = heavyIKSwitch;
            if (iKSetting == IKSetting.Pole) iKSwitch = poleIKSwitch;
            weight = Mathf.Lerp(0, 1, 1f - Mathf.Cos(iKSwitch.localPosition.y * Mathf.PI * 0.5f));
        }
    }
    */
    private void OnAnimatorIK(int layerIndex)
    {
        if (iKSetting == IKSetting.Heavy)
        {
            if (hand == IKHelperHand.LeftHand)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, heavyHandEffector.position);

                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, heavyHandEffector.rotation);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                anim.SetIKPosition(AvatarIKGoal.RightHand, heavyHandEffector.position);

                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
                anim.SetIKRotation(AvatarIKGoal.RightHand, heavyHandEffector.rotation);
            }
        }
        else if (iKSetting == IKSetting.Pole)
        {
            if (hand == IKHelperHand.LeftHand)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, poleHandEffector.position);

                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, poleHandEffector.rotation);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                anim.SetIKPosition(AvatarIKGoal.RightHand, poleHandEffector.position);

                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
                anim.SetIKRotation(AvatarIKGoal.RightHand, poleHandEffector.rotation);
            }
        }
    }

    public void SwitchActiveTool()
    {
        if (iKCoroutine != null) StopCoroutine(iKCoroutine);
        if (equipManager.mainHand != null)
        {
            var weapon = equipManager.mainHand.weapon;
            if (weapon.weaponType == WeaponType.Heavy)
            {
                iKSetting = IKSetting.Heavy;
                iKCoroutine = StartCoroutine(SetIKWeight());
            }
            else if (weapon.weaponType == WeaponType.Pole)
            {
                iKSetting = IKSetting.Pole;
                iKCoroutine = StartCoroutine(SetIKWeight());
            }
            else iKSetting = IKSetting.None;
        }
        else
        {
            //Maybe have another check later on if I use this for bows as well
            iKSetting = IKSetting.None;
        }
    }

    private IEnumerator SetIKWeight()
    {
        while (iKSetting == IKSetting.Heavy)
        {
            weight = Mathf.Lerp(0, 1, 1f - Mathf.Cos(heavyIKSwitch.localPosition.y * Mathf.PI * 0.5f));
            yield return null;
        }

        while (iKSetting == IKSetting.Pole)
        {
            weight = Mathf.Lerp(0, 1, 1f - Mathf.Cos(poleIKSwitch.localPosition.y * Mathf.PI * 0.5f));
            yield return null;
        }
    }
}
public enum IKHelperHand { LeftHand = 0, RightHand = 1 };
public enum IKSetting { None, Heavy, Pole }