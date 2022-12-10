using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* As this script works now, only damage Effects will register 
 I think that's the way it's going to stay, for instantiating "Wall" and "Storm" like spells*/

//I suppose this will be some sort of summoning spell, not necessarily conjuration, but it would fall under that effect
public class SpellNova : ActiveSpell
{
    [SerializeField] private AudioSource audioSource;
    public float duration;
    private List<EffectHolder> damageEffects = new List<EffectHolder>();

    public void OnSpawn(CharacterSpellcasting newCaster, EffectHolder[] newEffects, string name)
    {
        caster = newCaster;
        effects = newEffects;
        spellName = name;

        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].spellEffect is DamageEffect)
            {
                damageEffects.Add(effects[i]);
            }
        }

        if (damageEffects.Count == 0) ObjectPooler.ReturnToPool_Static("spellNova", gameObject);

        if (newEffects.Length == 0)
        {
            Debug.LogWarning("Empty projectile");
            ObjectPooler.ReturnToPool_Static("spellNova", gameObject);
        }

        if (activeAFX != null)
        {
            audioSource.clip = activeAFX;
            audioSource.Play();
        }
        StartCoroutine(SpellDuration(duration));
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger)
        {
            if (damageEffects.Count > 0)
            {
                var damageable = other.GetComponentInParent<IDamageable>();
                if (damageable != null) DealDamage(damageable);
            }
        }
    }

    protected virtual void DealDamage(IDamageable target)
    {
        for (int i = 0; i < damageEffects.Count; i++)
        {
            if (damageEffects[i].spellEffect is DamageEffect damage)
            {
                float totalDamage = (damageEffects[i].magnitude) * Time.fixedDeltaTime;
                target.ApplyDamage(caster.stats, totalDamage, damage.type, true);
            }
        }
    }

    IEnumerator SpellDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        ObjectPooler.ReturnToPool_Static("spellNova", gameObject);
    }
}
