using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReflection : MonoBehaviour
{
    CharacterStats stats;
    public float reflectedPercent = 0.1f;
    public DamageType reflectedDamageType;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponentInParent<CharacterStats>();
        stats.onDamageTaken += ReflectDamage;
        StartCoroutine(Duration());
    }

    public void ReflectDamage(CharacterStats attacker, float amount, DamageType type, bool isLethal)
    {
        DamageType reflect = reflectedDamageType;
        if (reflectedDamageType == DamageType.NULL) reflect = type;

        attacker.ApplyDamage(stats, amount * reflectedPercent, reflect, isLethal);
        Debug.Log("Damage Reflected");
    }

    IEnumerator Duration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(this);
    }
}
