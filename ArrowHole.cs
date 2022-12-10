using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHole : DamageSource, IEffectReceiver
{
    [SerializeField] private Outline outline;
    [SerializeField] private Trigger[] triggers;
    [SerializeField] private ParticleSystem firedDartEffect;
    [SerializeField] private Transform rayStartPoint;

    private float lastActivation;
    [SerializeField] private float triggerCooldown;

    private void Start()
    {
        outline.enabled = false;

        if (triggers != null)
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].onTriggerCallback += ActivateTrap;
            }

        }
    }

    [ContextMenu("Activate Trap")]
    private void ActivateTrap()
    {
        if (triggerCooldown > lastActivation)
        {
            firedDartEffect.Play();
            RaycastHit hit;
            Ray ray = new Ray(rayStartPoint.position, transform.forward);
            if (Physics.Raycast(ray, out hit))
            {
                var target = hit.collider.GetComponentInParent<IDamageable>();
                if (target != null) DamageTarget(target);
            }
            lastActivation = Time.time;
        }
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
        ActivateTrap();
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
