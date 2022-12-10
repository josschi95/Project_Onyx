using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAnimHelper : MonoBehaviour
{
    public GameObject arrow;
    public Animator anim;
    AudioHelper audioHelper;
    [SerializeField] AudioSource audioSource;

    private void Awake()
    {
        audioHelper = AudioHelper.instance;
        anim = GetComponent<Animator>();
    }

    public void SetDrawWeight(float weight)
    {
        anim.SetFloat("drawWeight", weight);
    }

    public void DrawArrow()
    {
        arrow.SetActive(true);
        audioSource.PlayOneShot(audioHelper.drawBow);
    }

    public void ReleaseArrow()
    {
        arrow.SetActive(false);
    }
}
