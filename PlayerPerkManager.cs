using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerkManager : MonoBehaviour
{
    #region - Singleton -
    public static PlayerPerkManager instance;

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerSkills found");
            return;
        }
        instance = this;
    }
    #endregion

    public PerkManager perkManager;
    private List<Perk> playerPerks = new List<Perk>();

    public void AddPerk(Perk perk)
    {
        playerPerks.Add(perk);
        perkManager.ApplyPerkBonuses(perk);
    }

    public bool HasPerk(Perk perk)
    {
        if (playerPerks.Contains(perk)) return true;
        return false;
    }
}
