using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to store all player data including stats and skills, inventory, quest progress, etc.
/// </summary>
public class PlayerData
{
    public string saveFileName; //The default name for the file
    public string saveFileNameOverride;  //If the player chooses to name a save file directly. We'll see if I implement this

    #region - Player General Info -
    public string pName;
    public string pArch;
    public string pClass;
    public string pBack;
    public string pBirth;
    public int age;
    public string pVirt;
    public string pVice;
    public string pHobby;
    public int pLife;
    #endregion

    #region - General -
    public string currentScene;
    public float[] position;
    public float[] rotation;
    public string playerLocation;

    public float timeOfDay;
    public int dayOfWeek;
    public int dayOfMonth;
    public int currentMonth;
    public int year;
    public int totalDayCount;
    //
    public int playerLevel;
    public int playerXP;
    public int difficulty;
    #endregion

    #region - Player Model -
    public bool isMale;
    public float[] skinColor; //float[3] rgb
    public float[] hairColor;
    public float[] stubbleColor;
    public float[] scarColor;
    public float[] eyeColor;
    public float[] paintColor;
    public int[] facialModels;
    #endregion

    #region - Player Stats -
    public float currentHealth;
    public int maxHealth;
    public int healthRegenRate;
    
    public float currentMana;
    public int maxMana;
    public int manaRegenRate;
    
    public float currentStamina;
    public int maxStamina;
    public int staminaRegenRate;

    public int strength;
    public int finesse;
    public int endurance;
    public int social;
    public int intellect;
    //public Stat discipline; //focus, 

    #region - Resistances -
    public int bludgeoningResistance;
    public int piercingResistance;
    public int slashingResistance;

    public int fireResistance;
    public int iceResistance;
    public int lightningResistance;

    public int holyResistance;
    public int poisonResistance;
    public int blightResistance;

    public int charmResistance;
    public int diseaseResistance;
    public int curseResistance;
    #endregion

    #region - Skills -
    /// <summary>
    /// For skills, I will also need to include the exp for each
    /// </summary>
    public int axes;
    public int axesXP;

    public int blades;
    public int bladesXP;

    public int clubs;
    public int clubsXP;

    public int heavyArmor;
    public int heavyArmorXP;

    public int polearms;
    public int polearmsXP;

    public int smithing;
    public int smithingXP;
    //
    public int evasion;
    public int evasionXP;

    public int guile;
    public int guileXP;

    public int lightArmor;
    public int lightArmorXP;

    public int ranged;
    public int rangedXP;

    public int stealth;
    public int stealthXP;

    public int thievery;
    public int thieveryXP;
    //
    public int alchemy;
    public int alchemyXP;

    public int mediumArmor;
    public int mediumArmorXP;

    public int necromancy;
    public int necromancyXP;

    public int striker;
    public int strikerXP;

    public int shields;
    public int shieldsXP;

    public int unarmored;
    public int unarmoredXP;
    //
    public int barter;
    public int barterXP;

    public int enchantment;
    public int enchantmentXP;

    public int intuition;
    public int intuitionXP;

    public int leadership;
    public int leadershipXP;

    public int linguistics;
    public int linguisticsXP;

    public int speech;
    public int speechXP;
    //
    public int abjuration;
    public int abjurationXP;

    public int conjuration;
    public int conjurationXP;

    public int divination;
    public int divinationXP;

    public int evocation;
    public int evocationXP;

    public int transmutation;
    public int transmutationXP;

    public int fabrication;
    public int fabricationXP;
    #endregion

    public List<ActiveEffect> activeEffects;

    #endregion

    #region - Spellcasting -
    public int casterDomain;
    //Divine
    public int playerCovenant;
    public int playerDevotion;
    #endregion

    #region - Inventory -
    public int copperPieces;
    public int silverPieces;
    public int goldPieces;

    public List<InventoryItem> allItems;
    public List<Spell> allSpells;
    public List<Ability> allAbilities;
    public List<Recipe> allRecipes;
    #endregion

    public int[] currentEquip;
    public bool usingSecondary;

    public List<NPC> knownNPCs;

    #region - Questing -
    public int curreneQuest;

    public List<int> activeQuests;
    public List<int> activeQuestStages;

    public List<int> completedQuests;
    public List<int> failedQuests;
    public List<int> abandonedQuests;
    #endregion

    public PlayerData (PlayerStats playerStats, PlayerSpellcasting casting, PlayerInventory inventory)
    {
        var manager = PlayerManager.instance;
        var equip = PlayerEquipmentManager.instance;

        #region - Player General Info
        pName = manager.playerName;
        pArch = manager.playerArchetype;
        pClass = manager.playerClass;
        pBack = manager.playerBackground;
        pBirth = manager.playerBirthplace;
        age = manager.playerAge;
        pVirt = manager.playerVirtues;
        pVice = manager.playerVices;
        pHobby = manager.playerHobbies;
        pLife = (int)manager.playerLifestyle;

        playerLevel = manager.playerLevel;
        playerXP = manager.totalExp;
        difficulty = GameMaster.instance.difficultyLevel;
        #endregion

        #region Player Model -
        isMale = equip.isMale;
        skinColor = new float[3];
        skinColor[0] = equip.skinColor.r;
        skinColor[1] = equip.skinColor.g;
        skinColor[2] = equip.skinColor.b;
        hairColor = new float[3];
        hairColor[0] = equip.hairColor.r;
        hairColor[1] = equip.hairColor.g;
        hairColor[2] = equip.hairColor.b;
        stubbleColor = new float[3];
        stubbleColor[0] = equip.stubbleColor.r;
        stubbleColor[1] = equip.stubbleColor.g;
        stubbleColor[2] = equip.stubbleColor.b;
        scarColor = new float[3];
        scarColor[0] = equip.scarColor.r;
        scarColor[1] = equip.scarColor.g;
        scarColor[2] = equip.scarColor.b;
        eyeColor = new float[3];
        eyeColor[0] = equip.eyeColor.r;
        eyeColor[1] = equip.eyeColor.g;
        eyeColor[2] = equip.eyeColor.b;
        paintColor = new float[3];
        paintColor[0] = equip.paintColor.r;
        paintColor[1] = equip.paintColor.g;
        paintColor[2] = equip.paintColor.b;
        facialModels = new int[4];
        equip.facialModels.CopyTo(facialModels, 0);
        #endregion

        #region - Date/Time/Location -
        currentScene = manager.GetCurrentScene();
        playerLocation = manager.playerLocation;

        position = new float[3];
        position[0] = playerStats.transform.position.x;
        position[1] = playerStats.transform.position.y;
        position[2] = playerStats.transform.position.z;
        rotation = new float[3];
        rotation[0] = playerStats.transform.rotation.eulerAngles.x;
        rotation[1] = playerStats.transform.rotation.eulerAngles.y;
        rotation[2] = playerStats.transform.rotation.eulerAngles.z;

        timeOfDay = DateTimeManager.instance.timeOfDay;
        dayOfWeek = (int)DateTimeManager.instance.day;
        dayOfMonth = DateTimeManager.instance.dayOfMonth;
        currentMonth = (int)DateTimeManager.instance.month;
        year = DateTimeManager.instance.year;
        totalDayCount = DateTimeManager.instance.totalDayCount;
        #endregion

        #region - Core -
        CharacterStatSheet statSheet = playerStats.statSheet;
        currentHealth = playerStats.currentHealth;
        maxHealth = statSheet.maxHealth.Base_Value;
        healthRegenRate = statSheet.healthRegenRate.Base_Value;

        currentMana = playerStats.currentMana;
        maxMana = statSheet.maxMana.Base_Value;
        manaRegenRate = statSheet.manaRegenRate.Base_Value;

        currentStamina = playerStats.currentStamina;
        maxStamina = statSheet.maxStamina.Base_Value;
        staminaRegenRate = statSheet.staminaRegenRate.Base_Value;

        strength = statSheet.strength.Base_Value;
        finesse = statSheet.finesse.Base_Value;
        endurance = statSheet.endurance.Base_Value;
        social = statSheet.social.Base_Value;
        intellect = statSheet.intellect.Base_Value;
        #endregion

        #region - Resistances -
        bludgeoningResistance = statSheet.bludgeoningResist.Base_Value;
        piercingResistance = statSheet.piercingResist.Base_Value;
        slashingResistance = statSheet.slashingResist.Base_Value;

        fireResistance = statSheet.fireResist.Base_Value;
        iceResistance = statSheet.iceResist.Base_Value;
        lightningResistance = statSheet.lightningResist.Base_Value;

        holyResistance = statSheet.holyResist.Base_Value;
        poisonResistance = statSheet.poisonResist.Base_Value;
        blightResistance = statSheet.blightResist.Base_Value;

        charmResistance = statSheet.charmResist.Base_Value;
        diseaseResistance = statSheet.diseaseResist.Base_Value;
        curseResistance = statSheet.curseResist.Base_Value;
        #endregion

        #region - Skills -

        #region - STR -
        axes = statSheet.axes.Base_Value;
        axesXP = statSheet.axes.currentXP;

        blades = statSheet.blades.Base_Value;
        bladesXP = statSheet.blades.currentXP;

        clubs = statSheet.clubs.Base_Value;
        clubsXP = statSheet.clubs.currentXP;

        heavyArmor = statSheet.heavyArmor.Base_Value;
        heavyArmorXP = statSheet.heavyArmor.currentXP;

        polearms = statSheet.polearms.Base_Value;
        polearmsXP = statSheet.polearms.currentXP;

        smithing = statSheet.smithing.Base_Value;
        smithingXP = statSheet.smithing.currentXP;
        #endregion

        #region - FIN -
        evasion = statSheet.evasion.Base_Value;
        evasionXP = statSheet.evasion.currentXP;

        guile = statSheet.guile.Base_Value;
        guileXP = statSheet.guile.currentXP;

        lightArmor = statSheet.lightArmor.Base_Value;
        lightArmorXP = statSheet.lightArmor.currentXP;

        ranged = statSheet.ranged.Base_Value;
        rangedXP = statSheet.ranged.currentXP;

        stealth = statSheet.stealth.Base_Value;
        stealthXP = statSheet.stealth.currentXP;

        thievery = statSheet.thievery.Base_Value;
        thieveryXP = statSheet.thievery.currentXP;
        #endregion

        #region - END -
        alchemy = statSheet.alchemy.Base_Value;
        alchemyXP = statSheet.alchemy.currentXP;

        mediumArmor = statSheet.mediumArmor.Base_Value;
        mediumArmorXP = statSheet.mediumArmor.currentXP;

        necromancy = statSheet.necromancy.Base_Value;
        necromancyXP = statSheet.necromancy.currentXP;

        striker = statSheet.striker.Base_Value;
        strikerXP = statSheet.striker.currentXP;

        shields = statSheet.shields.Base_Value;
        shieldsXP = statSheet.shields.currentXP;

        unarmored = statSheet.unarmored.Base_Value;
        unarmoredXP = statSheet.unarmored.currentXP;
        #endregion

        #region - SOC -
        barter = statSheet.barter.Base_Value;
        barterXP = statSheet.barter.currentXP;

        enchantment = statSheet.enchantment.Base_Value;
        enchantmentXP = statSheet.enchantment.currentXP;

        intuition = statSheet.intuition.Base_Value;
        intuitionXP = statSheet.intuition.currentXP;

        leadership = statSheet.leadership.Base_Value;
        leadershipXP = statSheet.leadership.currentXP;

        linguistics = statSheet.linguistics.Base_Value;
        linguisticsXP = statSheet.linguistics.currentXP;

        speech = statSheet.speech.Base_Value;
        speechXP = statSheet.speech.currentXP;
        #endregion

        #region - INT -
        abjuration = statSheet.abjuration.Base_Value;
        abjurationXP = statSheet.abjuration.currentXP;

        conjuration = statSheet.conjuration.Base_Value;
        conjurationXP = statSheet.conjuration.currentXP;

        divination = statSheet.divination.Base_Value;
        divinationXP = statSheet.divination.currentXP;

        evocation = statSheet.evocation.Base_Value;
        evocationXP = statSheet.evocation.currentXP;

        transmutation = statSheet.transmutation.Base_Value;
        transmutationXP = statSheet.transmutation.currentXP;

        fabrication = statSheet.fabrication.Base_Value;
        fabricationXP = statSheet.fabrication.currentXP;
        #endregion

        #endregion

        #region - Active Effects -
        activeEffects = new List<ActiveEffect>();
        activeEffects.AddRange(playerStats.activeEffects);
        #endregion

        #region - Spellcasting -
        casterDomain = (int)casting.casterDomain;
        playerCovenant = (int)casting.casterCovenant;
        playerDevotion = casting.divineDevotion;
        #endregion

        #region - Inventory -
        copperPieces = inventory.copperPieces;
        silverPieces = inventory.silverPieces;
        goldPieces = inventory.goldPieces;

        allItems = new List<InventoryItem>();
        allItems.AddRange(inventory.apparel);
        allItems.AddRange(inventory.weapons);
        allItems.AddRange(inventory.potions);
        //allItems.AddRange(inventory.crafting);
        allItems.AddRange(inventory.miscellaneous);

        allSpells = new List<Spell>();
        allSpells.AddRange(casting.arcaneSpellbook);

        allAbilities = new List<Ability>();
        allAbilities.AddRange(inventory.abilities);

        allRecipes = new List<Recipe>();
        allRecipes.AddRange(inventory.cookBook);
        #endregion

        #region - Equipment -
        usingSecondary = equip.usingSecondaryWeaponSet;
        currentEquip = new int[15];
        for (int i = 0; i < equip.currentEquipment.Length; i++)
        {
            if (equip.currentEquipment[i] != null)
                currentEquip[i] = equip.currentEquipment[i].itemID;
            else currentEquip[i] = -1;
        }
        #endregion

        #region - Quests -
        var qm = QuestManager.instance;

        curreneQuest = -1;
        if (qm.currentTrackedQuest != null)
            curreneQuest = qm.currentTrackedQuest.questID;

        activeQuests = new List<int>();
        for (int i = 0; i < qm.activeQuests.Count; i++)
        {
            activeQuests.Add(qm.activeQuests[i].questID);
        }

        activeQuestStages = new List<int>();
        for (int i = 0; i < qm.activeQuests.Count; i++)
        {
            activeQuestStages.Add(qm.activeQuests[i].currentStateNum);
        }

        completedQuests = new List<int>();
        for (int i = 0; i < qm.completedQuests.Count; i++)
        {
            completedQuests.Add(qm.completedQuests[i].questID);
        }

        failedQuests = new List<int>();
        for (int i = 0; i < qm.failedQuests.Count; i++)
        {
            failedQuests.Add(qm.failedQuests[i].questID);
        }

        abandonedQuests = new List<int>();
        for (int i = 0; i < qm.abandonedQuests.Count; i++)
        {
            abandonedQuests.Add(qm.abandonedQuests[i].questID);
        }
        #endregion

        knownNPCs = new List<NPC>();
        //Yet to be implemented

        saveFileName = manager.playerName + " Level " + manager.playerLevel + " " + manager.playerClass;
    }
}

//Player Perks
//Just keep a list of all perks and re-add them same as others
//I think I already have this as the "MasterPerkList" I just need to reformat it to be in line with everything else

//Current Equipment
//Also track the player's current quickslot items
//Quickslot Potions
//Quickslot Spells
//Quickslot Abilities

//Any Weapon Poisons

//Known NPCs
//Only need to track their affinity. If they're in the journal, the player has met them
//Ok also a list of the journal entries uncovered about them
//And then keep a separate list of dead NPCs, and cull them on start

//Bestiary and Crafting in journal
//IDK, I haven't even implemented this yet so I don't need to worry about it yet