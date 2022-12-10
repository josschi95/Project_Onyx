using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/Perk")]
public class Perk : ScriptableObject
{
    new public string name;
    [TextArea(1, 2)] public string description;
    [TextArea(2, 3)] public string editorNotes;

    [Header("Requirement")]
    public int reqPlayerLevel;
    [Space]
    public bool hasCoreRequirement;
    public CoreAttribute coreAttribute;
    public int requiredCoreValue;
    [Space]
    public bool hasMajorRequirement;
    public MajorAttribute majorAttribute;
    public int requiredMajorValue;
    [Space]
    public bool hasResistRequirement;
    public TypeResistance resistanceType;
    public int requiredResistValue;
    [Space]
    public bool hasSkillRequirement;
    public SkillField skillType;
    public int requiredSkillValue;

    //When the player levels up and is able to choose a new perk, use this for each perk to check which ones to display
    public bool PerkRequirementsMet()
    {
        if (PlayerManager.instance.playerLevel < reqPlayerLevel) return false;

        PlayerStats stats = PlayerStats.instance;

        if (hasCoreRequirement == true)
        {
            if (stats.statSheet.GetCoreAttribute(coreAttribute).Base_Value < requiredCoreValue) return false;
        }

        if (hasMajorRequirement == true)
        {
            if (stats.statSheet.GetMajorAttribute(majorAttribute).Base_Value < requiredMajorValue) return false;
        }

        if (hasResistRequirement == true)
        {
            if (stats.statSheet.GetResistance(resistanceType).Base_Value < requiredResistValue) return false;
        }

        if (hasSkillRequirement == true)
        {
            if (stats.statSheet.GetSkill(skillType).Base_Value < requiredSkillValue) return false;
        }
        return true;
    }
}
