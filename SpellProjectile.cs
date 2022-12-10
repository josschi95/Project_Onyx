using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : ActiveSpell
{
    public float speed = 20;
    [SerializeField] Rigidbody rb;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Collider coll;
    [Space]
    public ParticleSystem[] collisionEffects;
    private Coroutine despawnCoroutine;
    private float despawnDelay = 20f;
    [Space]
    [SerializeField] private ParticleSystem[] spellFX;

    public void OnSpawn(CharacterSpellcasting newCaster, EffectHolder[] newEffects, string name)
    {
        caster = newCaster;
        effects = newEffects;
        spellName = name;
        coll.enabled = true;
        if (newEffects.Length == 0)
        {
            Debug.LogWarning("Empty projectile");
            ObjectPooler.ReturnToPool_Static("spellProjectile", gameObject);
        }

        StopFX();
        spellFX[(int)caster.casterDomain].Play();

        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        if (activeAFX != null)
        {
            audioSource.clip = activeAFX;
            audioSource.Play();
        }
        despawnCoroutine = StartCoroutine(DespawnDelay(despawnDelay));
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            CharacterSpellcasting hit = other.GetComponentInParent<CharacterSpellcasting>();
            if (hit != null && hit == caster) return;
            OnContact(other);
        }
    }

    protected virtual void OnContact(Collider other)
    {
        if (effects[0].area >= 1)
        {
            Debug.Log("Collecting in Area");
            caster.CollectTargetsInRadius(transform, effects[0].area, effects, spellName);
        }
        else
        {
            Debug.Log("Hit " + other.transform.name);
            var target = other.GetComponent<IEffectReceiver>();
            if (target != null)
            {
                Debug.Log("Target found: " + other.transform.name);
                target.TransferEffects(effects, spellName);
            }
        }

        DisableProjectile();
        audioSource.Stop();
        if (impactAFX != null) audioSource.PlayOneShot(impactAFX);
        for (int i = 0; i < collisionEffects.Length; i++) collisionEffects[i].Play();        
    }

    private void DisableProjectile()
    {
        rb.velocity = Vector3.zero;
        coll.enabled = false;
        StopFX();
        if (despawnCoroutine != null) StopCoroutine(despawnCoroutine);
        despawnCoroutine = StartCoroutine(DespawnDelay(10));
    }

    private IEnumerator DespawnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPooler.ReturnToPool_Static("spellProjectile", gameObject);
    }

    private void StopFX()
    {
        for (int i = 0; i < spellFX.Length; i++)
        {
            spellFX[i].Stop();
        }
    }
}
