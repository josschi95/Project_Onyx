using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_PressurePlate : Trigger, IEffectReceiver
{
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Outline outline;
    [SerializeField] private AudioClip triggerSound;

    private void Start()
    {
        outline.enabled = false;
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            CharacterController character = other.GetComponent<CharacterController>();
            if (character != null)
            {
                OnTriggerEvent();
            }
        }
    }

    public override void OnTriggerEvent()
    {
        if (triggerSound != null)
        {
            audioSource.clip = triggerSound;
            audioSource.Play();
        }

        base.OnTriggerEvent();
    }

    #region - Spell Reception -
    public void TransferEffects(EffectHolder[] effects, string sourceName)
    {
        foreach (EffectHolder effect in effects)
        {
            if (effect.spellEffect is Telekinesis)
                OnTelekinesisSpell();
        }
    }

    private void OnTelekinesisSpell()
    {
        OnTriggerEvent();
    }

    public void OnDivinationEnter(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Traps)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red;
        }
    }

    public void OnDivinationExit(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Traps)
        {
            outline.enabled = false;
        }
    }
    #endregion
}
