using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyAnimator : MonoBehaviour
{
    public Destructable destructable;
    [SerializeField] Animator[] anims;
    [SerializeField] Animation[] anim;

    void Start()
    {
        destructable.onDestroyCallback += PlayAnims;
    }

    private void PlayAnims()
    {
        for (int i = 0; i < anims.Length; i++)
        {
            string triggerName = anims[i].GetParameter(0).name;
            anims[i].SetTrigger(triggerName);
        }

        for (int i = 0; i < anim.Length; i++)
        {
            anim[i].Play();
        }
    }
}
