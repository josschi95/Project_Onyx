using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpellcasting : CharacterSpellcasting, IEffectReceiver
{
    [Header("NPC Properties")]
    [SerializeField] private NPCController controller;
    public bool canCastSpells = false;

    public override void CastProjectileSpell(EffectHolder[] effects, string sourceName)
    {
        Vector3 target = controller.target.center.position;
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        GameObject newProjectile = ObjectPooler.SpawnFromPool_Static("spellProjectile", combat.strikingPoint.position, lookRotation);
        newProjectile.transform.LookAt(target);

        SpellProjectile projectile = newProjectile.GetComponent<SpellProjectile>();
        projectile.OnSpawn(this, effects, sourceName);
    }
}
