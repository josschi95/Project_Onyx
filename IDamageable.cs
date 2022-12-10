using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    //void ApplyWeaponDamage(CharacterStats attacker, Weapon weapon, float amount, DamageType type, bool isLethal);
    //Will have stagger/stun/knockdown effects which won't be available to spells

    void ApplyDamage(CharacterStats attacker, float amount, DamageType type, bool isLethal);

    void ApplyDamageOverTime(float amount, DamageType type, bool isLethal, float duration);
}

public enum PhyDmgType { Mundane, ColdIron, Silver, Magic }
