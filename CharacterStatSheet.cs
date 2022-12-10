using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stat Sheet", menuName = "Characters/Stat Sheet")]
[System.Serializable]
public class CharacterStatSheet : ScriptableObject
{
    public CharacterClass characterClass;
    public CharacterBackground background;
    [Space]
    #region - Stat Arrays -
    [Tooltip("55 in total, if not, something is wrong")]
    public Stat[] allStats;

    [HideInInspector] public Stat[] coreStatsArray;// = { maxHealth, maxMana, maxStamina, healthRegenRate, manaRegenRate, staminaRegenRate };
    [HideInInspector] public Stat[] attributesArray;
    [HideInInspector] public Stat[] resistancesArray;// = { bludgeoningResist, piercingResist, slashingResist, fireResist, iceResist, lightningResist, holyResist, poisonResist, blightResist, charmResist, diseaseResist, curseResist };
    [HideInInspector] public Stat[] allSkills;

    [HideInInspector] public Stat[] strengthSkills;
    [HideInInspector] public Stat[] finesseSkills;
    [HideInInspector] public Stat[] enduranceSkills;
    [HideInInspector] public Stat[] socialSkills;
    [HideInInspector] public Stat[] intellectSkills;
    #endregion

    #region - Stats - 
    [Header("Core Attributes")]
    public Stat maxHealth; //derived from... archetype? level up bonus is based on archetype? 5/7/10
    public Stat healthRegenRate;
    [Space]
    public Stat maxMana; //derived from intellect
    public Stat manaRegenRate;
    [Space]
    public Stat maxStamina; //derived from endurance
    public Stat staminaRegenRate;

    [Header("Major Attributes")]
    public Stat strength;
    public Stat finesse; //agility
    public Stat endurance; //fortitude, vitality, 
    public Stat social; //presence
    public Stat intellect; //knowledge
    //public Stat discipline; //focus, 

    [Header("Minor Attributes")]
    public Stat armor;
    public Stat carryCapacity;

    #region - Resistances -
    [Header("Resistances")]
    public Stat bludgeoningResist;
    public Stat piercingResist;
    public Stat slashingResist;
    public Stat fireResist;
    public Stat iceResist;
    public Stat lightningResist;
    public Stat holyResist;
    public Stat poisonResist;
    public Stat blightResist;
    [Space]
    public Stat charmResist;
    public Stat diseaseResist;
    public Stat curseResist;
    #endregion

    #region - Skills -
    [Header("Strength Skills")]
    public Skill axes;
    public Skill blades;
    public Skill clubs;
    public Skill heavyArmor;
    public Skill polearms;
    public Skill smithing;

    [Header("Finesse Skills")]
    public Skill evasion;
    public Skill guile;
    public Skill lightArmor;
    public Skill ranged;
    public Skill stealth;
    public Skill thievery;

    [Header("Endurance Skills")]
    public Skill alchemy;
    public Skill mediumArmor;
    public Skill necromancy;
    public Skill striker;
    public Skill shields;
    public Skill unarmored;

    [Header("Social Skills")]
    public Skill barter;
    public Skill enchantment;
    public Skill intuition;
    public Skill leadership;
    public Skill linguistics;
    public Skill speech;

    [Header("Intellect Skills")]
    public Skill abjuration;
    public Skill conjuration;
    public Skill divination;
    public Skill evocation;
    public Skill fabrication;
    public Skill transmutation;
    #endregion

    #endregion

    public List<Perk> perks;

    #region - Stat Modification -
    public void AddStatModifier(Stat stat, int magnitude)
    {
        stat.AddModifier(magnitude);
    }

    public void AddNewStatModifier(int statIndex, int magnitude)
    {
        allStats[statIndex].AddModifier(magnitude);
    }

    public void RemoveStatModifier(Stat stat, int magnitude)
    {
        stat.RemoveModifier(magnitude);
    }

    public void RemoveNewStatModifier(int statIndex, int magnitude)
    {
        allStats[statIndex].RemoveModifier(magnitude);
    }
    #endregion

    #region - Stat Array Compilation -
    public void CompileAllStats()
    {
        CompileCoreStats();
        CompileAttributeArray();
        CompileResistanceArray();
        CompileSkillArrays();

        List<Stat> newList = new List<Stat>();
        newList.AddRange(coreStatsArray);
        newList.AddRange(attributesArray);
        newList.AddRange(resistancesArray);
        newList.AddRange(allSkills);

        newList.Add(armor);
        newList.Add(carryCapacity);

        allStats = newList.ToArray();
    }

    private void CompileCoreStats()
    {
        coreStatsArray = new Stat[6];
        coreStatsArray[0] = maxHealth;
        coreStatsArray[1] = maxMana;
        coreStatsArray[2] = maxStamina;
        coreStatsArray[3] = healthRegenRate;
        coreStatsArray[4] = manaRegenRate;
        coreStatsArray[5] = staminaRegenRate;
    }

    private void CompileAttributeArray()
    {
        attributesArray = new Stat[5];

        attributesArray[0] = strength;
        attributesArray[1] = finesse;
        attributesArray[2] = endurance;
        attributesArray[3] = social;
        attributesArray[4] = intellect;
    }

    private void CompileResistanceArray()
    {
        Stat[] statArray = new Stat[12];

        statArray[0] = bludgeoningResist;
        statArray[1] = piercingResist;
        statArray[2] = slashingResist;

        statArray[3] = fireResist;
        statArray[4] = iceResist;
        statArray[5] = lightningResist;

        statArray[6] = holyResist;
        statArray[7] = poisonResist;
        statArray[8] = blightResist;

        statArray[9] = charmResist;
        statArray[10] = diseaseResist;
        statArray[11] = curseResist;

        resistancesArray = statArray;
    }

    private void CompileSkillArrays()
    {
        //Strength
        strengthSkills = new Stat[6];
        strengthSkills[0] = axes;
        strengthSkills[1] = blades;
        strengthSkills[2] = clubs;
        strengthSkills[3] = heavyArmor;
        strengthSkills[4] = polearms;
        strengthSkills[5] = smithing;

        //Finesse
        finesseSkills = new Stat[6];
        finesseSkills[0] = evasion;
        finesseSkills[1] = guile;
        finesseSkills[2] = lightArmor;
        finesseSkills[3] = ranged;
        finesseSkills[4] = stealth;
        finesseSkills[5] = thievery;

        //Endurance
        enduranceSkills = new Stat[6];
        enduranceSkills[0] = alchemy;
        enduranceSkills[1] = mediumArmor;
        enduranceSkills[2] = necromancy;
        enduranceSkills[3] = shields;
        enduranceSkills[4] = striker;
        enduranceSkills[5] = unarmored;

        //Social
        socialSkills = new Stat[6];
        socialSkills[0] = barter;
        socialSkills[1] = enchantment;
        socialSkills[2] = intuition;
        socialSkills[3] = leadership;
        socialSkills[4] = linguistics;
        socialSkills[5] = speech;

        //Intellect
        intellectSkills = new Stat[6];
        intellectSkills[0] = abjuration;
        intellectSkills[1] = conjuration;
        intellectSkills[2] = divination;
        intellectSkills[3] = evocation;
        intellectSkills[4] = fabrication;
        intellectSkills[5] = transmutation;

        allSkills = new Stat[30];
        strengthSkills.CopyTo(allSkills, 0);
        finesseSkills.CopyTo(allSkills, 6);
        enduranceSkills.CopyTo(allSkills, 12);
        socialSkills.CopyTo(allSkills, 18);
        intellectSkills.CopyTo(allSkills, 24);
    }
    #endregion

    #region - Stat Resets -
    public void ResetScores()
    {
        ResetAllSkills();
        ResetAllResistances();

        maxHealth.SetBaseValue(100);
        maxMana.SetBaseValue(100);
        maxStamina.SetBaseValue(100);

        healthRegenRate.SetBaseValue(0);
        armor.SetBaseValue(0);
        carryCapacity.SetBaseValue(3 * strength.GetValue());

        for (int i = 0; i < allStats.Length; i++)
        {
            allStats[i].modifiers.Clear();
        }
    }

    private void ResetAllSkills()
    {
        for (int i = 0; i < strengthSkills.Length; i++)
            strengthSkills[i].SetBaseValue(5 + strength.GetStatDecim());
        for (int i = 0; i < finesseSkills.Length; i++)
            finesseSkills[i].SetBaseValue(5 + finesse.GetStatDecim());
        for (int i = 0; i < enduranceSkills.Length; i++)
            enduranceSkills[i].SetBaseValue(5 + endurance.GetStatDecim());
        for (int i = 0; i < socialSkills.Length; i++)
            socialSkills[i].SetBaseValue(5 + social.GetStatDecim());
        for (int i = 0; i < intellectSkills.Length; i++)
            intellectSkills[i].SetBaseValue(5 + intellect.GetStatDecim());
    }

    private void ResetAllResistances()
    {
        for (int i = 0; i < resistancesArray.Length; i++)
            resistancesArray[i].SetBaseValue(0);
    }


    #endregion

    #region - Stat Reference -

    //Retrieve Stat Index
    public int GetCoreStatIndex(CoreAttribute stat)
    {
        return (int)stat;
    }

    public int GetAttributeIndex(MajorAttribute stat)
    {
        return coreStatsArray.Length + (int)stat;
    }

    public int GetResistanceIndex(TypeResistance resist)
    {
        return coreStatsArray.Length + attributesArray.Length + (int)resist;
    }

    public int GetSkillIndex(SkillField skill)
    {
        return coreStatsArray.Length + attributesArray.Length + resistancesArray.Length + (int)skill;
    }

    public int GetMinorAttributeIndex(MinorAttribute stat)
    {
        return coreStatsArray.Length + attributesArray.Length + resistancesArray.Length + allSkills.Length + (int)stat;
    }

    //Retrieve Stat Direct //Not recommended
    public Stat GetCoreAttribute(CoreAttribute stat)
    {
        return coreStatsArray[(int)stat];
    }

    public Stat GetMajorAttribute(MajorAttribute stat)
    {
        return attributesArray[(int)stat];
    }

    public Stat GetMinorAttribute(MinorAttribute stat)
    {
        if (stat == MinorAttribute.Armor) return armor;
        else return carryCapacity;
    }
    
    public Stat GetResistance(TypeResistance resist)
    {
        return resistancesArray[(int)resist];
    }

    public Stat GetSkill(SkillField skill)
    {
        return allSkills[(int)skill];
    }
    #endregion

    public void SetSavedValues(PlayerData data)
    {
        CompileAllStats();
        ResetScores();

        #region - Core -
        maxHealth.SetBaseValue(data.maxHealth);
        healthRegenRate.SetBaseValue(data.healthRegenRate);

        maxMana.SetBaseValue(data.maxMana);
        manaRegenRate.SetBaseValue(data.manaRegenRate);

        maxStamina.SetBaseValue(data.maxStamina);
        staminaRegenRate.SetBaseValue(data.staminaRegenRate);

        strength.SetBaseValue(data.strength);
        finesse.SetBaseValue(data.finesse);
        endurance.SetBaseValue(data.endurance);
        social.SetBaseValue(data.social);
        intellect.SetBaseValue(data.intellect);
        #endregion

        //armor will be set through equipment
        //carrycapacity should be recalculated 
        carryCapacity.SetBaseValue(strength.GetValue() * 3);

        #region - Resistances - 
        bludgeoningResist.SetBaseValue(data.bludgeoningResistance);
        piercingResist.SetBaseValue(data.piercingResistance);
        slashingResist.SetBaseValue(data.slashingResistance);

        fireResist.SetBaseValue(data.fireResistance);
        iceResist.SetBaseValue(data.iceResistance);
        lightningResist.SetBaseValue(data.lightningResistance);

        holyResist.SetBaseValue(data.holyResistance);
        poisonResist.SetBaseValue(data.poisonResistance);
        blightResist.SetBaseValue(data.blightResistance);

        charmResist.SetBaseValue(data.charmResistance);
        diseaseResist.SetBaseValue(data.diseaseResistance);
        curseResist.SetBaseValue(data.curseResistance);
        #endregion

        #region - STR -
        axes.SetSavedValue(data.axes, data.axesXP);
        blades.SetSavedValue(data.blades, data.bladesXP);
        clubs.SetSavedValue(data.clubs, data.clubsXP);
        heavyArmor.SetSavedValue(data.heavyArmor, data.heavyArmorXP);
        polearms.SetSavedValue(data.polearms, data.polearmsXP);
        smithing.SetSavedValue(data.smithing, data.smithingXP);
        #endregion

        #region - FIN -
        evasion.SetSavedValue(data.evasion, data.evasionXP);
        guile.SetSavedValue(data.guile, data.guileXP);
        lightArmor.SetSavedValue(data.lightArmor, data.lightArmorXP);
        ranged.SetSavedValue(data.ranged, data.rangedXP);
        stealth.SetSavedValue(data.stealth, data.stealthXP);
        thievery.SetSavedValue(data.thievery, data.thieveryXP);
        #endregion

        #region - END -
        alchemy.SetSavedValue(data.alchemy, data.alchemyXP);
        mediumArmor.SetSavedValue(data.mediumArmor, data.mediumArmorXP);
        necromancy.SetSavedValue(data.necromancy, data.necromancyXP);
        shields.SetSavedValue(data.shields, data.shieldsXP);
        striker.SetSavedValue(data.striker, data.strikerXP);
        unarmored.SetSavedValue(data.unarmored, data.unarmoredXP);
        #endregion

        #region - SOC -
        barter.SetSavedValue(data.barter, data.barterXP);
        enchantment.SetSavedValue(data.enchantment, data.enchantmentXP);
        intuition.SetSavedValue(data.intuition, data.intuitionXP);
        leadership.SetSavedValue(data.leadership, data.leadershipXP);
        linguistics.SetSavedValue(data.linguistics, data.linguisticsXP);
        speech.SetSavedValue(data.speech, data.speechXP);
        #endregion

        #region - INT -
        abjuration.SetSavedValue(data.abjuration, data.abjurationXP);
        conjuration.SetSavedValue(data.conjuration, data.conjuration);
        divination.SetSavedValue(data.divination, data.divinationXP);
        evocation.SetSavedValue(data.evocation, data.evocationXP);
        fabrication.SetSavedValue(data.fabrication, data.fabricationXP);
        transmutation.SetSavedValue(data.transmutation, data.transmutationXP);
        #endregion
    }

    public void ApplyClassToSheet()
    {
        if (characterClass != null)
        {
            characterClass.ApplyClassPresets(this);
        }
        else Debug.Log("Need to Assign Character Class");
    }

    public void ApplyBackgroundToSheet()
    {
        if (background != null)
        {
            characterClass.ApplyClassPresets(this);
        }
        else Debug.Log("Need to Assign Character Class");
    }
}

#region - enums -
public enum CoreAttribute { Health, Stamina, Mana, HealthRegen, ManaRegen, StaminaRegen }
public enum MajorAttribute { Strength, Finesse, Endurance, Social, Intellect }
public enum MinorAttribute { Armor, Carry_Capacity }
public enum TypeResistance 
{ 
    Bludg, Pierce, Slash, 
    Fire, Ice, Lightning, 
    Holy, Poison, Blight, 
    Charm, Disease, Curse 
}
public enum SkillField
{
    Axes, Blades, Clubs, Heavy_Armor, Polearms, Smithing, 
    Evasion, Guile, Light_Armor, Ranged, Stealth, Thievery, 
    Alchemy, Medium_Armor, Necromancy, Shields, Striker, Unarmored, 
    Barter, Enchantment, Intuition, Leadership, Linguistics, Speech, 
    Abjuration, Conjuration, Divination, Evocation, Fabrication, Transmutation
}
public enum DamageType { Bludgeoning, Piercing, Slashing, Fire, Ice, Lightning, Holy, Poison, Blight, NULL }
#endregion


/*private void AssignNames()
{
    maxHealth.statName = "maxHealth";
    healthRegenRate.statName = "healthRegenRate";

    maxMana.statName = "maxMana";
    manaRegenRate.statName = "manaRegenRate";

    maxStamina.statName = "maxStamina";
    staminaRegenRate.statName = "staminaRegenRate";

    strength.statName = "strength";
    finesse.statName = "finesse";
    endurance.statName = "endurance";
    social.statName = "social";
    intellect.statName = "intellect";

    armor.statName = "armor";
    carryCapacity.statName = "carryCapacity";

    bludgeoningResistance.statName = "bludgeoningResistance";
    piercingResistance.statName = "piercingResistance";
    slashingResistance.statName = "slashingResistance";
    fireResistance.statName = "fireResistance";
    iceResistance.statName = "iceResistance";
    lightningResistance.statName = "lightningResistance";
    holyResistance.statName = "holyResistance";
    poisonResistance.statName = "poisonResistance";
    blightResistance.statName = "blightResistance";
    charmResistance.statName = "charmResistance";
    diseaseResistance.statName = "diseaseResistance";
    curseResistance.statName = "curseResistance";


    axes.statName = "axes";
    blades.statName = "blades";
    clubs.statName = "clubs";
    heavyArmor.statName = "heavyArmor";
    polearms.statName = "polearms";
    smithing.statName = "smithing";

    evasion.statName = "evasion";
    guile.statName = "guile";
    lightArmor.statName = "lightArmor";
    ranged.statName = "ranged";
    stealth.statName = "stealth";
    thievery.statName = "thievery";

    alchemy.statName = "alchemy";
    mediumArmor.statName = "mediumArmor";
    necromancy.statName = "necromancy";
    striker.statName = "striker";
    shields.statName = "shields";
    unarmored.statName = "unarmored";

    barter.statName = "barter";
    enchantment.statName = "enchantment";
    intuition.statName = "intuition";
    leadership.statName = "leadership";
    linguistics.statName = "linguistics";
    speech.statName = "speech";

    abjuration.statName = "abjuration";
    conjuration.statName = "conjuration";
    divination.statName = "divination";
    evocation.statName = "evocation";
    transmutation.statName = "transmutation";
    fabrication.statName = "fabrication";
    
}*/