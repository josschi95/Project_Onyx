using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CharacterPanelManager : MonoBehaviour
{
    #region - Singleton -
    public static CharacterPanelManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of CharacterPanelManager found");
            return;
        }

        instance = this;
    }
    #endregion

    private PlayerManager playerManager;
    private PlayerStats playerStats;
    private CharacterStatSheet playerStatSheet;
    private PlayerSpellcasting spellcasting; //I can include some info about domain here maybe

    [SerializeField] private Button skillsButton, perksButton;
    [SerializeField] private GameObject skillsPanel, perksPanel;
    [Space]
    [SerializeField] private TMP_Text playerName; 
    [SerializeField] private TMP_Text playerLevel, health, stamina, mana;
    [SerializeField] private Slider healthBar, staminaBar, manaBar;
    [SerializeField] private TMP_Text strength, finesse, endurance, social, intellect;
    [Header("Resistances")]
    [SerializeField] private TMP_Text bludg;
    [SerializeField] private TMP_Text pierce, slash, fire, ice, lightning, holy, poison, blight, charm, disease, curse;
    [Header("Player Skills")]
    [SerializeField] private TMP_Text axes;
    [SerializeField] private TMP_Text blades, clubs, heavyArmor, polearms, smithing;
    [Space]
    [SerializeField] private TMP_Text evasion;
    [SerializeField] private TMP_Text guile, lightArmor, ranged, stealth, thievery;
    [Space]
    [SerializeField] private TMP_Text alchemy;
    [SerializeField] private TMP_Text mediumArmor, necromancy, striker, shields, unarmored;
    [Space]
    [SerializeField] private TMP_Text barter;
    [SerializeField] private TMP_Text enchantment, intuition, leadership, linguistics, speech;
    [Space]
    [SerializeField] private TMP_Text abjuration;
    [SerializeField] private TMP_Text conjuration, divination, evocation, fabrication, transmutation;
    [Header("Skill XP Sliders")]
    [SerializeField] private Slider axesSlider;
    [SerializeField] private Slider bladesSlider, clubsSlider, heavyArmorSlider, polearmsSlider, smithingSlider;
    [Space]
    [SerializeField] private Slider evasionSlider;
    [SerializeField] private Slider guileSlider, lightArmorSlider, rangedSlider, stealthSlider, thieverySlider;
    [Space]
    [SerializeField] private Slider alchemySlider;
    [SerializeField] private Slider mediumArmorSlider, necromancySlider, strikerSlider, shieldsSlider, unarmoredSlider;
    [Space]
    [SerializeField] private Slider barterSlider;
    [SerializeField] private Slider enchantmentSlider, intuitionSlider, leadershipSlider, linguisticsSlider, speechSlider;
    [Space]
    [SerializeField] private Slider abjurationSlider;
    [SerializeField] private Slider conjurationSlider, divinationSlider, evocationSlider, fabricationSlider, transmutationSlider;

    private void Start()
    {
        playerStats = PlayerStats.instance;
        playerManager = PlayerManager.instance;
        spellcasting = PlayerSpellcasting.instance;
        playerStatSheet = playerStats.statSheet;

        skillsButton.onClick.AddListener(delegate { DisplaySkills(); });
        perksButton.onClick.AddListener(delegate { DisplayPerksPanel(); });
    }

    public void UpdateMenu()
    {
        playerName.text = playerManager.playerName;
        playerLevel.text = "Level: " + playerManager.playerLevel.ToString();

        health.text = "Health: " + Mathf.RoundToInt(playerStats.currentHealth) + " / " + Mathf.RoundToInt(playerStatSheet.maxHealth.GetValue());
        stamina.text = "Stamina: " + Mathf.RoundToInt(playerStats.currentStamina) + " / " + Mathf.RoundToInt(playerStatSheet.maxStamina.GetValue());
        mana.text = "Mana: " + Mathf.RoundToInt(playerStats.currentMana) + " / " + Mathf.RoundToInt(playerStatSheet.maxMana.GetValue());

        int maxHPValue = playerStatSheet.maxHealth.GetValue();
        healthBar.maxValue = maxHPValue;
        healthBar.value = playerStats.currentHealth;

        int maxSPValue = playerStatSheet.maxStamina.GetValue();
        staminaBar.maxValue = maxSPValue;
        staminaBar.value = playerStats.currentStamina;

        int maxValue = playerStatSheet.maxMana.GetValue();
        manaBar.maxValue = maxValue;
        manaBar.value = playerStats.currentMana;
        //Later add some changes so if GetValue() is greater than baseValue, it's green, red if lower

        strength.text = playerStatSheet.strength.GetValue().ToString();
        finesse.text = playerStatSheet.finesse.GetValue().ToString();
        endurance.text = playerStatSheet.endurance.GetValue().ToString();
        social.text = playerStatSheet.social.GetValue().ToString();
        intellect.text = playerStatSheet.intellect.GetValue().ToString();

        DisplayResistances();
        DisplaySkills();
        //DisplaySkillsXP();
    }

    private void DisplayResistances()
    {
        bludg.text = playerStatSheet.bludgeoningResist.GetValue().ToString() + "%";
        pierce.text = playerStatSheet.piercingResist.GetValue().ToString() + "%";
        slash.text = playerStatSheet.slashingResist.GetValue().ToString() + "%";

        fire.text = playerStatSheet.fireResist.GetValue().ToString() + "%";
        ice.text = playerStatSheet.iceResist.GetValue().ToString() + "%";
        lightning.text = playerStatSheet.lightningResist.GetValue().ToString() + "%";

        holy.text = playerStatSheet.holyResist.GetValue().ToString() + "%";
        poison.text = playerStatSheet.poisonResist.GetValue().ToString() + "%";
        blight.text = playerStatSheet.blightResist.GetValue().ToString() + "%";

        charm.text = playerStatSheet.charmResist.GetValue().ToString() + "%";
        curse.text = playerStatSheet.curseResist.GetValue().ToString() + "%";
        disease.text = playerStatSheet.diseaseResist.GetValue().ToString() + "%";
    }

    private void DisplaySkills()
    {
        skillsPanel.SetActive(true);
        perksPanel.SetActive(false);

        //STR
        axes.text = playerStatSheet.axes.GetValue().ToString();
        blades.text = playerStatSheet.blades.GetValue().ToString();
        clubs.text = playerStatSheet.clubs.GetValue().ToString();
        heavyArmor.text = playerStatSheet.heavyArmor.GetValue().ToString();
        polearms.text = playerStatSheet.polearms.GetValue().ToString();
        smithing.text = playerStatSheet.smithing.GetValue().ToString();
        //FIN
        evasion.text = playerStatSheet.evasion.GetValue().ToString();
        guile.text = playerStatSheet.guile.GetValue().ToString();
        lightArmor.text = playerStatSheet.lightArmor.GetValue().ToString();
        ranged.text = playerStatSheet.ranged.GetValue().ToString();
        stealth.text = playerStatSheet.stealth.GetValue().ToString();
        thievery.text = playerStatSheet.thievery.GetValue().ToString();
        //END
        alchemy.text = playerStatSheet.alchemy.GetValue().ToString();
        mediumArmor.text = playerStatSheet.mediumArmor.GetValue().ToString();
        necromancy.text = playerStatSheet.necromancy.GetValue().ToString();
        striker.text = playerStatSheet.striker.GetValue().ToString();
        shields.text = playerStatSheet.shields.GetValue().ToString();
        unarmored.text = playerStatSheet.unarmored.GetValue().ToString();
        //SOC
        barter.text = playerStatSheet.barter.GetValue().ToString();
        enchantment.text = playerStatSheet.enchantment.GetValue().ToString();
        intuition.text = playerStatSheet.intuition.GetValue().ToString();
        leadership.text = playerStatSheet.leadership.GetValue().ToString();
        linguistics.text = playerStatSheet.linguistics.GetValue().ToString();
        speech.text = playerStatSheet.speech.GetValue().ToString();
        //INT
        abjuration.text = playerStatSheet.abjuration.GetValue().ToString();
        conjuration.text = playerStatSheet.conjuration.GetValue().ToString();
        divination.text = playerStatSheet.divination.GetValue().ToString();
        evocation.text = playerStatSheet.evocation.GetValue().ToString();
        fabrication.text = playerStatSheet.fabrication.GetValue().ToString();
        transmutation.text = playerStatSheet.transmutation.GetValue().ToString();
    }

    private void DisplaySkillsXP()
    {
        //STR
        axesSlider.maxValue = playerStatSheet.axes.xPToNextLevel;
        axesSlider.value = playerStatSheet.axes.currentXP;
        
        bladesSlider.maxValue = playerStatSheet.blades.xPToNextLevel;
        bladesSlider.value = playerStatSheet.blades.currentXP;
        
        clubsSlider.maxValue = playerStatSheet.clubs.xPToNextLevel;
        clubsSlider.value = playerStatSheet.clubs.currentXP;
        
        heavyArmorSlider.maxValue = playerStatSheet.heavyArmor.xPToNextLevel;
        heavyArmorSlider.value = playerStatSheet.heavyArmor.currentXP;
        
        polearmsSlider.maxValue = playerStatSheet.polearms.xPToNextLevel;
        polearmsSlider.value = playerStatSheet.polearms.currentXP;
        
        smithingSlider.maxValue = playerStatSheet.smithing.xPToNextLevel;
        smithingSlider.value = playerStatSheet.smithing.currentXP;

        //FIN
        evasionSlider.maxValue = playerStatSheet.evasion.xPToNextLevel;
        evasionSlider.value = playerStatSheet.evasion.currentXP;
        
        guileSlider.maxValue = playerStatSheet.guile.xPToNextLevel;
        guileSlider.value = playerStatSheet.guile.currentXP;
        
        lightArmorSlider.maxValue = playerStatSheet.lightArmor.xPToNextLevel;
        lightArmorSlider.value = playerStatSheet.lightArmor.currentXP;
       
        rangedSlider.maxValue = playerStatSheet.ranged.xPToNextLevel;
        rangedSlider.value = playerStatSheet.ranged.currentXP;
        
        stealthSlider.maxValue = playerStatSheet.stealth.xPToNextLevel;
        stealthSlider.value = playerStatSheet.stealth.currentXP;
        
        thieverySlider.maxValue = playerStatSheet.thievery.xPToNextLevel;
        thieverySlider.value = playerStatSheet.thievery.currentXP;

        //END
        alchemySlider.maxValue = playerStatSheet.alchemy.xPToNextLevel;
        alchemySlider.value = playerStatSheet.alchemy.currentXP;
        
        mediumArmorSlider.maxValue = playerStatSheet.mediumArmor.xPToNextLevel;
        mediumArmorSlider.value = playerStatSheet.mediumArmor.currentXP;
        
        necromancySlider.maxValue = playerStatSheet.necromancy.xPToNextLevel;
        necromancySlider.value = playerStatSheet.necromancy.currentXP;
        
        strikerSlider.maxValue = playerStatSheet.striker.xPToNextLevel;
        strikerSlider.value = playerStatSheet.striker.currentXP;
        
        shieldsSlider.maxValue = playerStatSheet.shields.xPToNextLevel;
        shieldsSlider.value = playerStatSheet.shields.currentXP;
        
        unarmoredSlider.maxValue = playerStatSheet.unarmored.xPToNextLevel;
        unarmoredSlider.value = playerStatSheet.unarmored.currentXP;

        //SOC
        barterSlider.maxValue = playerStatSheet.barter.xPToNextLevel;
        barterSlider.value = playerStatSheet.barter.currentXP;
        
        enchantmentSlider.maxValue = playerStatSheet.enchantment.xPToNextLevel;
        enchantmentSlider.value = playerStatSheet.enchantment.currentXP;
        
        intuitionSlider.maxValue = playerStatSheet.intuition.xPToNextLevel;
        intuitionSlider.value = playerStatSheet.intuition.currentXP;
        
        leadershipSlider.maxValue = playerStatSheet.leadership.xPToNextLevel;
        leadershipSlider.value = playerStatSheet.leadership.currentXP;
        
        linguisticsSlider.maxValue = playerStatSheet.linguistics.xPToNextLevel;
        linguisticsSlider.value = playerStatSheet.linguistics.currentXP;
        
        speechSlider.maxValue = playerStatSheet.speech.xPToNextLevel;
        speechSlider.value = playerStatSheet.speech.currentXP;

        //INT
        abjurationSlider.maxValue = playerStatSheet.abjuration.xPToNextLevel;
        abjurationSlider.value = playerStatSheet.abjuration.currentXP;
        
        conjurationSlider.maxValue = playerStatSheet.conjuration.xPToNextLevel;
        conjurationSlider.value = playerStatSheet.conjuration.currentXP;
        
        divinationSlider.maxValue = playerStatSheet.divination.xPToNextLevel;
        divinationSlider.value = playerStatSheet.divination.currentXP;
        
        evocationSlider.maxValue = playerStatSheet.evocation.xPToNextLevel;
        evocationSlider.value = playerStatSheet.evocation.currentXP;
        
        fabricationSlider.maxValue = playerStatSheet.fabrication.xPToNextLevel;
        fabricationSlider.value = playerStatSheet.fabrication.currentXP;
        
        transmutationSlider.maxValue = playerStatSheet.transmutation.xPToNextLevel;
        transmutationSlider.value = playerStatSheet.transmutation.currentXP;
    }

    #region - Panels Right Toggle-
    private void DisplayPerksPanel()
    {
        skillsPanel.SetActive(false);
        perksPanel.SetActive(true);
    }
    #endregion
}
