using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public CharacterCombat combat;

    public Weapon projectileWeapon;
    [Space]
    [SerializeField] Collider coll;
    [SerializeField] Rigidbody rb;
    public ParticleSystem normalWeaponTrail;
    [SerializeField] AudioSource audioSource;

    private float deathDelay = 20f;
    public float soundRadius = 1;
    [Range(10f, 100f)]
    public float speed = 20;
    Coroutine deathCoroutine;

    [Header("Weapon Poison")]
    public Poison poison;

    public List<EffectHolder> bonusEffects = new List<EffectHolder>();
    private bool autoReturn;
    private bool returned;

    public void OnSpawn(CharacterCombat combat, Weapon weapon, Poison poison, bool autoReturn)
    {
        this.combat = combat;
        projectileWeapon = weapon;
        this.poison = poison;
        this.autoReturn = autoReturn;
    }

    private void Start()
    {
        coll.enabled = true;
        rb.velocity = transform.forward * speed;
        audioSource.PlayOneShot(AudioHelper.instance.fireArrow);
        if (normalWeaponTrail != null) normalWeaponTrail.Play(true);
        deathCoroutine = StartCoroutine(DestroySelf(deathDelay));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            CharacterCombat hit = other.GetComponentInParent<CharacterCombat>();
            if (hit != null && hit == combat) return;

            IDamageable newTarget = other.GetComponentInParent<IDamageable>();
            if (newTarget != null)
            {
                OnProjectileHit(other, newTarget);
                RecoverAmmo(other);
            }
            else
            {
                //Play arrow hitting stone/ground/wood sound FX
            }
            DisableProjectile(other);
        }
    }

    //The projectile strikes an object, disable it
    protected virtual void DisableProjectile(Collider other)
    {
        normalWeaponTrail.Stop();

        rb.velocity = Vector3.zero;
        coll.enabled = false;
        GetComponentInChildren<MeshRenderer>().enabled = false;
        //This just makes it invisble. It would be cool to have it correctly parent to the target somehow, but that's probably a lot of code. 
        SoundPing();
        if (deathCoroutine != null) StopCoroutine(deathCoroutine);
        deathCoroutine = StartCoroutine(DestroySelf(10f));
    }

    protected virtual void OnProjectileHit(Collider other, IDamageable damageable)
    {
        audioSource.clip = AudioHelper.instance.projectileImpact;
        audioSource.Play();

        var damage = combat.GetProjectileDamage(projectileWeapon);
        damageable.ApplyDamage(combat.characterStats, damage.magnitude, damage.type, projectileWeapon.isLethal);

        CharacterStats character = other.GetComponentInParent<CharacterStats>();
        if (character != null && poison != null) ApplyPoison(character);
        for (int i = 0; i < bonusEffects.Count; i++)
        {
            bonusEffects[i].ApplyStatEffect(character, projectileWeapon.name);
        }
    }

    public void ApplyPoison(CharacterStats character)
    {
        if (character != null)
        {
            for (int i = 0; i < poison.potionEffects.Count; i++)
            {
                poison.potionEffects[i].ApplyStatEffect(character, poison.name);
            }
        }
    }

    private void RecoverAmmo(Collider other)
    {
        //Return spell effect ensures ammo recovery
        if (autoReturn)
        {
            combat.characterController.inventory.AddItem(projectileWeapon, 1);
            returned = true;
            return;
        }

        Container box = other.GetComponent<Container>();
        if (box != null)
        {
            if (combat.projectileReturn == true)
            {
                box.AddItem(projectileWeapon, 1);
                returned = true;
                return;
            }
            int target = 50;
            if (PlayerPerkManager.instance.HasPerk(PlayerPerkManager.instance.perkManager.ammunitionRecovery)) target = 75;
            int num = Random.Range(0, 101);
            if (num <= target)
            {
                box.AddItem(projectileWeapon, 1);
                returned = true;
            }
        }
    }

    //Change this to return to pool
    private void DestroyOjbect()
    {
        if (returned == false && autoReturn == true)
        {
            combat.characterController.inventory.AddItem(projectileWeapon, 1);
            returned = true;
        }
        Destroy(gameObject);
    }

    private IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyOjbect();
    }

    private void SoundPing()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].isTrigger == false)
            {
                var enemy = colliders[i].GetComponent<NPCController>();
                if (enemy != null)
                    enemy.OnSoundDetected(transform.position);
            }
        }
    }
}
