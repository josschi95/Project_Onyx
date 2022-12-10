using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionLock : StateMachineBehaviour
{
    public bool triggerOnEnter = false;
    public bool triggerOnExit = false;
    public bool acceptInput = false;
    public bool interacting = false;
    public bool disableUpperBody = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (disableUpperBody) animator.SetLayerWeight(1, 0);
        if (triggerOnEnter == true)
        {
            if (acceptInput)
            {
                animator.GetComponentInParent<CharacterController>().acceptInput = false;
            }
            else if (interacting)
            {
                animator.GetComponentInParent<CharacterController>().isInteracting = true;
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (disableUpperBody) animator.SetLayerWeight(1, 1);
        if (triggerOnExit == true)
        {
            if (acceptInput)
            {
                animator.GetComponentInParent<CharacterController>().acceptInput = true;
            }
            else if (interacting)
            {
                animator.GetComponentInParent<CharacterController>().isInteracting = false;
            }
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
