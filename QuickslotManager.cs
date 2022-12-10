using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickslotManager : MonoBehaviour
{
    #region - Singleton - 
    public static QuickslotManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of QuickslotManager found");
            return;
        }
        instance = this;
    }
    #endregion

    Player_Settings settings;

    public List<Potion> preparedPotions = new List<Potion>();
    public List<Spell> preparedSpells = new List<Spell>();
    public List<Ability> preparedAbilities = new List<Ability>();
    public List<Item> preparedRelics = new List<Item>();
    [Space]
    private Coroutine toggleCoroutine;
    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    public RectTransform quickSlotParent;
    public QuickSlot_Ability abilitySlot;
    public QuickSlot_Potion potionSlot;
    public QuickSlot_Relic relicSlot;
    public QuickSlot_Spell spellSlot;

    private int activeSlotNum = 0;

    private int abilitySlotNum = 0;
    private int potionSlotNum = 0;
    [HideInInspector] public int spellSlotNum = 0;
    private int relicSlotNum = 0;

    private void Start()
    {
        settings = Player_Settings.instance;
        settings.onSettingsChanged += OnSettingsChange;

        shownPosition = quickSlotParent.anchoredPosition;
        hiddenPosition = shownPosition;
        hiddenPosition.y -= 200;
    }

    private void OnSettingsChange()
    {

    }

    public void ToggleQuickslots(bool hide)
    {
        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);
        toggleCoroutine = StartCoroutine(ToggleQuickslotsCoroutine(hide));
    }

    public void HideImmediate()
    {
        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);
        quickSlotParent.anchoredPosition = hiddenPosition;
    }


    #region - Quick Slot Navigation -
    public void OnActiveSlotChange(bool right)
    {
        if (right) activeSlotNum++;
        else activeSlotNum--;

        if (activeSlotNum > 3) activeSlotNum = 0;
        else if (activeSlotNum < 0) activeSlotNum = 3;

        if (activeSlotNum == 0)
        {
            potionSlot.background.enabled = true;
            abilitySlot.background.enabled = false;
            spellSlot.background.enabled = false;
            relicSlot.background.enabled = false;
        }
        else if (activeSlotNum == 1)
        {
            potionSlot.background.enabled = false;
            abilitySlot.background.enabled = true;
            spellSlot.background.enabled = false;
            relicSlot.background.enabled = false;
        }
        else if (activeSlotNum == 2)
        {
            potionSlot.background.enabled = false;
            abilitySlot.background.enabled = false;
            spellSlot.background.enabled = true;
            relicSlot.background.enabled = false;
        }
        else if (activeSlotNum == 3)
        {
            potionSlot.background.enabled = false;
            abilitySlot.background.enabled = false;
            spellSlot.background.enabled = false;
            relicSlot.background.enabled = true;
        }
    }

    public void CycleActiveSlot(bool up)
    {
        //Potions
        if (activeSlotNum == 0) OnCyclePotions(up);
        //Abilities
        else if (activeSlotNum == 1) OnCycleAbilities(up);
        //Spells
        else if (activeSlotNum == 2) OnCycleSpells(up);
        //Relics
        else if (activeSlotNum == 3) OnCycleRelics(up);
    }

    public void UseActiveSlot()
    {
        //Potions
        if (activeSlotNum == 0) potionSlot.OnUse();
        //Abilities
        else if (activeSlotNum == 1) abilitySlot.OnUse();
        //Spells
        //else if (activeSlotNum == 2) //spellSlot.OnUse(); //this does nothing
        //Relics
        else if (activeSlotNum == 3) relicSlot.OnUse();
    }
    #endregion

    #region - Abilities
    private void OnCycleAbilities(bool up)
    {
        if (up) abilitySlotNum++;
        else abilitySlotNum--;

        if (abilitySlotNum >= preparedAbilities.Count)
        {
            abilitySlotNum = 0;
        }
        else if (abilitySlotNum < 0)
        {
            abilitySlotNum = preparedAbilities.Count - 1;
        }
        if (preparedAbilities.Count != 0)
            abilitySlot.OnAssignAbility(preparedAbilities[abilitySlotNum]);
    }

    public void OnAbilityChange(Ability ability)
    {
        if (ability != null)
        {
            if (preparedAbilities.Contains(ability))
                preparedAbilities.Remove(ability);
            else
                preparedAbilities.Add(ability);
        }

        //Can I get rid of all of this and only include the if (abilitySlotNum >= prepared.Count
        if (preparedAbilities.Count == 0)
        {
            abilitySlot.OnAssignAbility(null);
        }
        else if (abilitySlotNum >= preparedAbilities.Count)
        {
            abilitySlotNum = preparedAbilities.Count - 1;
            abilitySlot.OnAssignAbility(preparedAbilities[abilitySlotNum]);
        }
        else
        {
            abilitySlot.OnAssignAbility(preparedAbilities[abilitySlotNum]);
        }
    }

    public void SetActiveAbility(Ability ability)
    {
        if (preparedAbilities.Contains(ability) == false) OnAbilityChange(ability);
        if (abilitySlot.ability != ability)
        {
            for (int i = 0; i < preparedAbilities.Count; i++)
            {
                if (preparedAbilities[i] == ability)
                {
                    abilitySlotNum = i;
                    abilitySlot.OnAssignAbility(preparedAbilities[abilitySlotNum]);
                    break;
                }
            }
        }
    }
    #endregion

    #region - Potions -
    private void OnCyclePotions(bool up)
    {
        if (up) potionSlotNum++;
        else potionSlotNum--;

        if (potionSlotNum >= preparedPotions.Count)
        {
            potionSlotNum = 0;
        }

        else if (potionSlotNum < 0)
        {
            //I don't know why this would get called, but I guess have it just in case
            potionSlotNum = preparedPotions.Count - 1;
        }

        if (preparedPotions.Count != 0)
            potionSlot.OnAssignPotion(preparedPotions[potionSlotNum]);
    }

    public void OnPotionsChange(Potion potion)
    {
        if (potion != null)
        {
            if (preparedPotions.Contains(potion))
                preparedPotions.Remove(potion);
            else
            {
                if (preparedPotions.Count >= 4)
                {
                    preparedPotions[3] = potion;
                }
                else preparedPotions.Add(potion);
            }
        }

        if (preparedPotions.Count == 0)
        {
            potionSlot.OnAssignPotion(null);
        }
        else if (potionSlotNum >= preparedPotions.Count)
        {
            potionSlotNum = preparedPotions.Count - 1;
            potionSlot.OnAssignPotion(preparedPotions[potionSlotNum]);
        }
        else
        {
            potionSlot.OnAssignPotion(preparedPotions[potionSlotNum]);
        }
    }
    #endregion

    #region - Relics -
    private void OnCycleRelics(bool up)
    {
        if (up) relicSlotNum++;
        else relicSlotNum--;

        if (relicSlotNum >= preparedRelics.Count)
        {
            relicSlotNum = 0;
        }
        else if (relicSlotNum < 0)
        {
            relicSlotNum = preparedRelics.Count - 1;
        }
        if (preparedRelics.Count != 0)
            relicSlot.OnAssignRelic(preparedRelics[relicSlotNum]);
    }

    public void OnRelicChange(Item relic)
    {
        if (relic != null)
        {
            if (preparedRelics.Contains(relic))
                preparedRelics.Remove(relic);
            else
            {
                if (preparedRelics.Count >= 4)
                {
                    preparedRelics[3] = relic;
                }
                else preparedRelics.Add(relic);
            }
        }

        if (preparedRelics.Count == 0)
        {
            relicSlot.OnAssignRelic(null);
        }
        else if (relicSlotNum >= preparedRelics.Count)
        {
            relicSlotNum = preparedRelics.Count - 1;
            relicSlot.OnAssignRelic(preparedRelics[relicSlotNum]);
        }
        else
        {
            relicSlot.OnAssignRelic(preparedRelics[relicSlotNum]);
        }
    }
    #endregion

    #region - Spells - 
    private void OnCycleSpells(bool up)
    {
        if (up) spellSlotNum++;
        else spellSlotNum--;

        if (spellSlotNum >= preparedSpells.Count)
        {
            spellSlotNum = 0;
        }
        else if (spellSlotNum < 0)
        {
            spellSlotNum = preparedSpells.Count - 1;
        }
        if (preparedSpells.Count != 0)
            spellSlot.OnAssignSpell(preparedSpells[spellSlotNum]);
    }

    public void OnSpellChange(Spell spell)
    {
        if (spell != null)
        {
            if (preparedSpells.Contains(spell))
                preparedSpells.Remove(spell);
            else
                preparedSpells.Add(spell);
        }

        if (preparedSpells.Count == 0)
        {
            spellSlot.OnAssignSpell(null);
        }
        else if (spellSlotNum >= preparedSpells.Count)
        {
            spellSlotNum = preparedSpells.Count - 1;
            spellSlot.OnAssignSpell(preparedSpells[spellSlotNum]);
        }
        else
        {
            spellSlot.OnAssignSpell(preparedSpells[spellSlotNum]);
        }
    }

    public void SetActiveSpell(Spell spell)
    {
        if (preparedSpells.Contains(spell) == false) OnSpellChange(spell);
        if (spellSlot.spell != spell)
        {
            for (int i = 0; i < preparedSpells.Count; i++)
            {
                if (preparedSpells[i] == spell)
                {
                    spellSlotNum = i;
                    spellSlot.OnAssignSpell(preparedSpells[spellSlotNum]);
                    break;
                }
            }
        }
    }
    #endregion

    [SerializeField] private float timeToMove = 0.25f;
    private IEnumerator ToggleQuickslotsCoroutine(bool hide)
    {
        float elapsedTime = 0;
        var startPos = quickSlotParent.anchoredPosition;
        var endPos = shownPosition;

        if (hide == true) endPos = hiddenPosition;

        while (elapsedTime < timeToMove)
        {
            quickSlotParent.anchoredPosition = Vector2.Lerp(startPos, endPos, (elapsedTime / timeToMove));
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        quickSlotParent.anchoredPosition = endPos;
    }
}
