using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    List<IDamageable> damagedTargets = new List<IDamageable>();

    public int damage;
    public DamageType damageType;
    public float damageDelay = 1f;
    public bool playCritAnim = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            //this weapon strikes a character
            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target != null && damagedTargets.Contains(target) == false)
            {
                DamageTarget(target);
                StartCoroutine(ResetTarget(target));
                if (playCritAnim)
                {
                    var character = other.GetComponent<CharacterStats>();
                    if (character != null)
                    {
                        character.CriticalHitTaken();
                    }
                }
            }
        }
    }

    protected IEnumerator ResetTarget(IDamageable target)
    {
        yield return new WaitForSeconds(damageDelay);
        damagedTargets.Remove(target);
    }

    protected virtual void DamageTarget(IDamageable target)
    {
        target.ApplyDamage(null, damage, damageType, true);
    }
}
