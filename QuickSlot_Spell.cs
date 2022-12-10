using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot_Spell : QuickSlot
{
    private PlayerSpellcasting playerSpellcasting;
    public Spell spell; //Do I need this reference?

    private void Start()
    {
        playerSpellcasting = PlayerSpellcasting.instance;
        OnAssignSpell(null);
    }

    public void OnAssignSpell(Spell newSpell)
    {
        if (newSpell == null)
        {
            spell = null;
            slotImage.sprite = null;
            slotImage.enabled = false;
            slotName.text = "";
        }
        else
        {
            spell = newSpell;
            slotImage.enabled = true;
            slotImage.sprite = newSpell.icon;
            slotName.text = newSpell.name;
        }
        //playerSpellcasting.OnReadiedSpellChange(spell);
        playerSpellcasting.readiedSpell = spell;
    }
}
