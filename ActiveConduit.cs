using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveConduit : ActiveWeapon
{
    CharacterSpellcasting spellcasting;

    [SerializeField] ParticleSystem arcaneFX;
    [SerializeField] ParticleSystem divineFX;
    [SerializeField] ParticleSystem druidicFX;

    protected override void Start()
    {
        base.Start();
        spellcasting = GetComponentInParent<CharacterSpellcasting>();
    }

    public void PlaySpellReady()
    {
        ParticleSystem fx = null;

        if (spellcasting.casterDomain == MagicalDomain.Arcane)
        {
            fx = arcaneFX;
        }
        if (spellcasting.casterDomain == MagicalDomain.Divine)
        {
            fx = divineFX;
        }
        if (spellcasting.casterDomain == MagicalDomain.Druidic)
        {
            fx = druidicFX;
        }
        fx.Play();
    }

    public void StopEffect()
    {
        arcaneFX.Stop();
        divineFX.Stop();
        druidicFX.Stop();
    }
}