using System.Collections.Generic;
using UnityEngine;

public class CharacterSpellcasting : MonoBehaviour, IEffectReceiver
{
    protected AudioSource audioSource;
    public CharacterStats stats;
    [SerializeField] protected CharacterCombat combat;
    [SerializeField] protected EquipmentManager equipmentManager;
    [Space]
    public List<Spell> arcaneSpellbook = new List<Spell>();
    public MagicalDomain casterDomain;

    [Header("Divine Casting")]
    public Deities casterCovenant;
    public int divineDevotion;
    public List<Spell> divineSpellbook = new List<Spell>();

    [Space]
    /*[HideInInspector]*/ public ActiveConduit conduit;
    /*[HideInInspector]*/ public Spell readiedSpell; //the spell that will be cast
    [HideInInspector] public bool castingSpell = false;

    //This will likely be removed, spells can ONLY be self, touch, or target, not a combination
    protected List<EffectHolder> selfSpellEffects = new List<EffectHolder>();
    protected List<EffectHolder> touchSpellEffects = new List<EffectHolder>();
    protected List<EffectHolder> targetSpellEffects = new List<EffectHolder>();

    protected virtual void Start()
    {
        equipmentManager.onEquipmentChanged += delegate { UpdateConduit(); };
        audioSource = GetComponent<AudioSource>();
    }

    //Yeah this is going to have to go as well
    protected void UpdateConduit()
    {
        if (equipmentManager.mainHand != null && equipmentManager.mainHand is ActiveConduit conduitRight)
        {
            conduit = conduitRight;
        }
        else if (equipmentManager.offHand != null && equipmentManager.offHand is ActiveConduit conduitLeft)
        {
            conduit = conduitLeft;
        }
        else conduit = null;
    }

    #region - Spell Casting -
    //can cast without conduit with 60 int or 50 in relevant score
    public virtual bool CanCastWithoutConduit()
    {
        switch (casterDomain)
        {
            case MagicalDomain.Arcane:
                {
                    if (stats.statSheet.intellect.GetValue() >= 70) return true;
                    return false;
                }
            case MagicalDomain.Divine:
                {
                    if (divineDevotion >= 50) return true;
                    return false;
                }
            case MagicalDomain.Druidic:
                {
                    return true;
                }
        }
        return false;
    }

    public virtual void CastReadiedSpell()
    {
        //if (SuccessfulCasting(readiedSpell))
        CastSpell(readiedSpell.spellEffects.ToArray(), readiedSpell.name);
        stats.SpendMana(readiedSpell.manaCost);
    }

    protected virtual void CastSpell(EffectHolder[] effects, string sourceName)
    {
        selfSpellEffects.Clear();
        touchSpellEffects.Clear();
        targetSpellEffects.Clear();

        for (int i = 0; i < effects.Length; i++)
        {
            if (readiedSpell.spellEffects[i].effectRange == SpellRange.Self)
                selfSpellEffects.Add(effects[i]);
            if (readiedSpell.spellEffects[i].effectRange == SpellRange.Melee)
                touchSpellEffects.Add(effects[i]);
            if (readiedSpell.spellEffects[i].effectRange == SpellRange.Range)
                targetSpellEffects.Add(effects[i]);
        }

        if (selfSpellEffects.Count > 0)
        {
            CastSpellOnSelf(selfSpellEffects.ToArray(), sourceName);
        }
        if (touchSpellEffects.Count > 0)
        {
            CastTouchSpell(combat.strikingPoint, touchSpellEffects.ToArray(), sourceName);
        }
        if (targetSpellEffects.Count > 0)
        {
            CastProjectileSpell(targetSpellEffects.ToArray(), sourceName);
        }
        audioSource.PlayOneShot(effects[0].spellEffect.onCast);
    }

    #region - Spell Ranges -
    public virtual void CastSpellOnSelf(EffectHolder[] effects, string sourceName)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].ApplyStatEffect(stats, readiedSpell.name);
        }
    }

    //Call this directly from activeWeapon/Projectile for OnStrike effects
    public virtual void CastTouchSpell(Transform origin, EffectHolder[] effects, string sourceName)
    {
        Ray ray = new Ray(combat.strikingPoint.position, transform.forward);
        RaycastHit hit;
        Debug.DrawRay(combat.strikingPoint.position, transform.forward, Color.blue);
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            var target = hit.collider.GetComponentInParent<IEffectReceiver>();
            if (target != null)
            {
                target.TransferEffects(effects, readiedSpell.name);
            }
        }
    }

    public virtual void CastProjectileSpell(EffectHolder[] effects, string sourceName)
    {
        //Meant to be overwritten
    }

    //I'll implement this once I actually have everything else working
    public virtual void CastSpotSpell(EffectHolder[] effects, string sourceName)
    {
        //Meant to be overwritten
    }
    #endregion

    //This is called for area effects
    public virtual void CollectTargetsInRadius(Transform origin, float radius, EffectHolder[] effects, string sourceName)
    {
        Collider[] colls = Physics.OverlapSphere(origin.position, radius);
        foreach (Collider coll in colls)
        {
            coll.TryGetComponent(out IEffectReceiver receiver);
            receiver.TransferEffects(effects, sourceName);
        }
    }
    #endregion

    #region - Spell Reception -
    //Later add in some variables and functions for magic resistance/weakness, spell absorption, spell reflection
    public virtual void TransferEffects(EffectHolder[] effects, string sourceName)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            Debug.Log(effects[i].spellEffect.name);
            if (effects[i].spellEffect is StatEffect statEffect)
            {
                statEffect.ApplyEffect(stats, sourceName, effects[i].magnitude, effects[i].duration);
            }
        }
    }

    [SerializeField] Outline divinationEffect;
    public void OnDivinationEnter(DivinedObjects targetType)
    {
        //Meant to be overwritten
    }

    public void OnDivinationExit(DivinedObjects targetType)
    {
        //Meant to be overwritten
    }
    #endregion
}
public enum SpellShape { Self, Nova, Aura, Melee, Missile, Beam, Spot }
public enum SpellRange { Self, Melee, Range, Spot }
public enum MagicalDomain { Arcane, Divine, Druidic }
public enum SpellSchool { Abjuration, Conjuration, Divination, Enchantment, Evocation, Necromancy, Transmutation }
public enum Deities { none, justice, protection, peace, Manengaal_death, corruption, war }
public enum WorldRegion { Desert, Forest, Mountains, Ocean, Plains }


//Abjuration: Protection and Denial
//Conjuration: Creation and Summoning
//Divination: Knowledge and Sight
//Enchantment: Make things do stuff, add effects to normally mundane stuff
//Illusion: Make stuff seem like other stuff, mind-affecting magic
//Invocation: Destroy stuff
//Necromancy: Life/Death stuff includes majority of healing spells
//Transmutation: Change stuff into other stuff

/* 
 * from there, I can store values and functions for 
 * Weakness to Magic (Should not affect DamageEffects) but increases magnitude of incoming effects
 * Resistance to Magic
 * Spell Absorption
 *  Channel Mana: absorb spell mana cost and funnel it into MP
 *  Channel Health: absorb spell mana cost and funnel it into HP
 *  Channel Stamina: absorb spell mana cost and funnel it into SP
 * Spell Reflection, need reference to original caster
 * 
 * 
 * Spell Effects with a range of Touch are cast in the same exact way that an unarmed strike is made, with an overlap sphere used at the strike point with strike radius, small enough that only one target should be affected
 * For the player however, this is likely going to have to change slight to take into account camera angle, and will probably be a raycast instead, why not do same for both? 
 * If they raycast returns a hit, the effects are applied to that target. the area parameter of these effects does nothing
 * Note that for OnStrike runes, just use whatever collider the weapon collides with... of course this takes into account some other things, 
 *  and makes me think I should switch over to spherecast for weapons too, isntead of colliders
 *  
 *      [Header("Druidic Casting")]
    public int desertAffinity;
    public int forestAffinity;
    public int mountainAffinity;
    public int oceanAffinity;
    public int plainsAffinity;
*/