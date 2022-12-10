using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathTrap : DamageSource, IEffectReceiver
{
    [SerializeField] private Outline outline;
    [SerializeField] private Trigger[] triggers;
    [SerializeField] private Collider damageCollider;
    [SerializeField] private ParticleSystem breathEffect;
    [SerializeField] private float duration;
    public EffectHolder[] trapSpellEffects;
    [SerializeField] private float triggerCooldown;
    private bool canBeActivated = true;
    private List<IEffectReceiver> affectedChars = new List<IEffectReceiver>();

    private void Start()
    {
        if (triggers != null)
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].onTriggerCallback += ActivateTrap;
            }           
        }
        outline.enabled = false;
        damageCollider.enabled = false;
        breathEffect.Stop();
    }

    private void ActivateTrap()
    {
        if (canBeActivated)
        {
            affectedChars.Clear();
            canBeActivated = false;
            damageCollider.enabled = true;
            breathEffect.Play();
            StartCoroutine(SwitchOff());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (trapSpellEffects.Length == 0) return;

        if (other.isTrigger == false)
        {
            var receiver = other.GetComponent<IEffectReceiver>();
            if (receiver != null && affectedChars.Contains(receiver) == false)
            {
                affectedChars.Add(receiver);
                receiver.TransferEffects(trapSpellEffects, transform.name);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger == false)
        {
            var target = other.GetComponentInParent<IDamageable>();
            if (target != null) DamageTarget(target);
        }
    }

    protected override void DamageTarget(IDamageable target)
    {
        base.DamageTarget(target);
        target.ApplyDamage(null, damage * Time.fixedDeltaTime, damageType, true);
    }

    IEnumerator SwitchOff()
    {
        yield return new WaitForSeconds(duration);
        damageCollider.enabled = false;
        breathEffect.Stop();
        yield return new WaitForSeconds(triggerCooldown);
        canBeActivated = true;
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
