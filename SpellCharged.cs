using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCharged : StateMachineBehaviour
{
    CharacterSpellcasting spellcasting;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (spellcasting == null) spellcasting = animator.GetComponentInParent<CharacterSpellcasting>();
        spellcasting.conduit.PlaySpellReady();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        spellcasting.conduit.StopEffect();
    }
}
