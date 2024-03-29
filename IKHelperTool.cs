﻿///////////////////////////////////////////////////////////////////////////
//  IKHelperTool                                                         //
//  Kevin Iglesias - https://www.keviniglesias.com/     			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class IKHelperTool : MonoBehaviour
{
	public Transform iKSwitch;
    public Transform handEffector;
        
    public IKHelperHand hand;
        
	private Animator animator;
	private float weight;

	void Awake()
	{
		animator = GetComponent<Animator>();
		weight = 0f;
	}

	void Update()
	{
        weight = Mathf.Lerp(0, 1, 1f - Mathf.Cos(iKSwitch.localPosition.y * Mathf.PI * 0.5f));
	}
		
	void OnAnimatorIK(int layerIndex)
	{
        if(hand == IKHelperHand.LeftHand)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, handEffector.position);
                
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, handEffector.rotation);
        }else{
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, handEffector.position);
                
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKRotation(AvatarIKGoal.RightHand, handEffector.rotation);
        }
	}
}
