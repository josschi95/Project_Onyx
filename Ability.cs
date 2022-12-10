using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Ability")]
public class Ability : ScriptableObject
{
    public int abilityID;
    new public string name;
    public AbilityType type;
    public Sprite icon = null;
    [TextArea(3, 5)] public string description;
    public string flavorText = "";
    [Space]
    public float duration = 0f;
    public float cooldownDuration = 10f;
    public AnimationClip animClip;
    public AudioClip soundEffect;
    [Space]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private bool followsUser = true;
    public string particleName;
    public ParticleSystem particleEffects;
    public float radius;
    [Space]
    public List<EffectHolder> userOnlyEffects = new List<EffectHolder>();
    public List<EffectHolder> positiveEffects = new List<EffectHolder>();
    public List<EffectHolder> negativeEffects = new List<EffectHolder>();
    [Space]
    [TextArea(3, 5)] public string Notes;

    public virtual void Use(CharacterStats stats)
    {
        if (soundEffect != null)
            stats.GetComponent<AudioSource>().PlayOneShot(soundEffect);

        Vector3 newPos = stats.transform.position + positionOffset;
        if (particleName != "")
        {
            Transform point = null;
            if (followsUser == true) point = stats.transform;
            ObjectPooler.SpawnFromPool_Static(particleName, newPos, Quaternion.identity);
        }

        for (int i = 0; i < userOnlyEffects.Count; i++)
        {
            userOnlyEffects[i].ApplyStatEffect(stats, name);
        }

        Collider[] colls = Physics.OverlapSphere(newPos, radius);
        foreach (Collider coll in colls)
        {
            if (coll.isTrigger == false)
            {
                var character = coll.GetComponentInParent<CharacterStats>();
                if (character != null)
                {
                    if (character.characterController.team == stats.characterController.team)
                    {
                        ApplyPositiveEffects(character);
                    }
                    else if (stats.characterController.team.alliedTeams.Contains(character.characterController.team))
                    {
                        ApplyPositiveEffects(character);
                    }
                    else ApplyNegativeEffects(character);
                }
                    
            }
        }
    }

    public virtual void ApplyPositiveEffects(CharacterStats stats)
    {
        for (int i = 0; i < positiveEffects.Count; i++)
            positiveEffects[i].ApplyStatEffect(stats, name);
    }

    public virtual void ApplyNegativeEffects(CharacterStats stats)
    {
        for (int i = 0; i < negativeEffects.Count; i++)
            negativeEffects[i].ApplyStatEffect(stats, name);
    }

    public virtual void ReadyAbility()
    {
        HUDManager.instance.quickslotManager.SetActiveAbility(this);
    }

    //Add this ability to the prepared ability list
    public virtual void Prepare()
    {
        HUDManager.instance.quickslotManager.OnAbilityChange(this);
    }
}

public enum AbilityType { Defensive, Offensive, Buff }