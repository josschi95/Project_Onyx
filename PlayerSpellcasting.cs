using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellcasting : CharacterSpellcasting, IEffectReceiver
{
    #region - Singleton -
    public static PlayerSpellcasting instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerSpellcasting found");
            return;
        }
        instance = this;
    }
    #endregion

    public bool autoSucceed;

    private PlayerCombat playerCombat;
    private Animator anim;
    private Camera cam;

    public GameObject spellMarker;
    [HideInInspector] public bool showSpellMarker = false;

    public ParticleSystem[] spellFXLeft;
    public ParticleSystem[] spellFXRight;

    private Coroutine spellMarkerCoroutine;

    protected override void Start()
    {
        base.Start();
        playerCombat = GetComponent<PlayerCombat>();
        anim = GetComponentInChildren<Animator>();
        cam = CameraHelper.instance.mainCam;
    }

    public void LearnSpell(Spell spell)
    {
        if (arcaneSpellbook.Contains(spell))
            UIManager.instance.DisplayPopup(spell.name + " is already known");
        else
        {
            UIManager.instance.DisplayPopup("You learned the " + spell.name + " spell");
            arcaneSpellbook.Add(spell);
        }
    }

    public void QuickCast()
    {
        if (CanCastSpell() == false) return;

        playerCombat.castingSpell = true;
        anim.Play("spell_right_cast");
    }

    public void PrepareSpell()
    {
        if (CanCastSpell() == false) return;

        playerCombat.castingSpell = true;
        anim.SetBool("holdingSpell", true);

        //Activates the spell marker to show where the spell will be cast
        if (readiedSpell.useSpellMarker == true)
        {
            showSpellMarker = true;
            if (spellMarkerCoroutine != null) StopCoroutine(spellMarkerCoroutine);
            spellMarkerCoroutine = StartCoroutine(DisplaySpellMarker(readiedSpell));
        }
    }

    public void ReleaseSpell()
    {
        showSpellMarker = false;
        playerCombat.castingSpell = false;
        anim.SetBool("holdingSpell", false);
    }

    private bool CanCastSpell()
    {
        if (GameMaster.instance.gamePaused) return false;

        if (readiedSpell == null)
        {
            UIManager.instance.DisplayPopup("No Spell Readied");
            return false;
        }

        if (PlayerStats.instance.CheckManaCost(readiedSpell.manaCost) == false)
        {
            UIManager.instance.DisplayPopup("Not enough Mana");
            return false;
        }

        if (conduit == null && !CanCastWithoutConduit())
        {
            UIManager.instance.DisplayPopup("Cannot Cast Spell");
            return false;
        }

        if (combat.weaponsDrawn == false && !CanCastWithoutConduit())
        {
            UIManager.instance.DisplayPopup("Cannot Cast Spell");
            return false;
        }

        return true;
    }

    public override void CastReadiedSpell()
    {
        /*if (SuccessfulCasting(readiedSpell))
        {
        }
        else
        {
            UIManager.instance.DisplayPopup("Spellcasting Failed");
        }*/
        CastSpell(readiedSpell.spellEffects.ToArray(), readiedSpell.name);
        stats.SpendMana(readiedSpell.manaCost);
    }

    //Probably wind up re-implementing this for forward/upward casting
    /*public void OnReadiedSpellChange(Spell spell)
    {
        readiedSpell = spell;
        Debug.Log("Probably remove all of this");
        if (spell != null)
        {
            readiedSpell = spell;
            if (conduit != null)
            {
                if (conduit.equipSlot == EquipmentSlot.Primary_Main || conduit.equipSlot == EquipmentSlot.Secondary_Main)
                {
                    //playerAnim.OnSpellChange(spell, true);
                }
                else
                {
                    //playerAnim.OnSpellChange(spell, false);
                }
            }
        }
        else readiedSpell = null;
    }*/

    public override void CastProjectileSpell(EffectHolder[] effects, string sourceName)
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        GameObject newProjectile = ObjectPooler.SpawnFromPool_Static("spellProjectile", playerCombat.strikingPoint.position, transform.rotation);
        
        if (Physics.Raycast(ray, out hit, 100)) newProjectile.transform.LookAt(hit.point);

        SpellProjectile projectile = newProjectile.GetComponent<SpellProjectile>();
        projectile.OnSpawn(this, effects, sourceName);
    }

    public IEnumerator DisplaySpellMarker(Spell spell)
    {
        if (spell != null)
        {
            spellMarker.SetActive(true);
            while (showSpellMarker)
            {
                Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 target = hit.point;
                    Debug.DrawLine(cam.transform.position, target);
                    spellMarker.transform.position = target;
                }
                yield return null;
            }
            spellMarker.SetActive(false);
            //spellMarkerCoroutine = null;
            yield return new WaitForSeconds(1f);
            spellMarker.transform.position = transform.position;//Vector3.zero;
        }
    }

    /*public override bool SuccessfulCasting(Spell spell)
    {
        if (autoSucceed) return true;
        return base.SuccessfulCasting(spell);
    }*/
}