using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, IDamageable
{
    public delegate void OnDestroy();
    public OnDestroy onDestroyCallback;

    public int maxHP;
    public float currentHP;
    public bool indestructible = false;
    [Space]
    public ParticleSystem destroyedEffect;
    public GameObject[] disableOnDestroyed;

    private void Start()
    {
        currentHP = maxHP;
    }

    public virtual void ApplyDamage(CharacterStats attacker, float amount, DamageType type, bool isLethal)
    {
        if (indestructible == true || type == DamageType.Poison) return;

        currentHP -= amount;
        if (currentHP <= 0) DestroyObject();
    }

    public virtual void ApplyDamageOverTime(float amount, DamageType type, bool isLethal, float duration)
    {
        ApplyDamage(null, amount, type, isLethal);
    }

    [ContextMenu("Destroy Object")]
    public void DestroyObject()
    {
        if (destroyedEffect != null)
        {
            destroyedEffect.Play();
        }

        for (int i = 0; i < disableOnDestroyed.Length; i++)
        {
            disableOnDestroyed[i].SetActive(false);
        }

        //Do something
        if (onDestroyCallback != null)
        {
            onDestroyCallback.Invoke();
        }
    }
}
