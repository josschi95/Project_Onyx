using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Inventory/Spell")]
public class Spell : ScriptableObject
{
    public int spellID;
    new public string name;
    [Header("Spell Properties")]
    public Sprite icon = null;
    [Space]

    public SpellShape spellShape;
    public float duration;
    public float area;

    [Space]
    public MagicalDomain spellDomain;
    public SpellSchool favoredSchool;
    public float manaCost;
    public bool useSpellMarker { get; private set; }
    [Space]
    public List<EffectHolder> spellEffects = new List<EffectHolder>();
    [TextArea(3, 5)] [SerializeField] private string editorNotes;

    //Set this spell to the currently active spell
    public void ReadySpell()
    {
        HUDManager.instance.quickslotManager.SetActiveSpell(this);
    }

    //Add this spell to the prepared spell list
    public void Prepare()
    {
        HUDManager.instance.quickslotManager.OnSpellChange(this);
    }

    #region - Initial Score Settings -
    public void OnSpellCreation()
    {
        CalculateManaCost();

        useSpellMarker = false;
        for (int i = 0; i < spellEffects.Count; i++)
        {
            if (spellEffects[i].effectRange == SpellRange.Spot)
            {
                useSpellMarker = true;
                break;
            }
        }
    }
    
    private void CalculateManaCost()
    {
        float num = 0;
        for (int i = 0; i < spellEffects.Count; i++)
        {
            float effectCost = spellEffects[i].spellEffect.baseCost * (spellEffects[i].magnitude * (spellEffects[i].duration + 1) + spellEffects[i].area);
            if (spellEffects[i].effectRange == SpellRange.Range) effectCost *= 1.5f;
            else if (spellEffects[i].effectRange == SpellRange.Spot) effectCost *= 2f;
            num += effectCost;
        }
        manaCost = num;
    }
    #endregion
}