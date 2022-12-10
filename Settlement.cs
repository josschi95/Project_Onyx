using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Settlement", menuName = "Locations/Settlement")]
public class Settlement : ScriptableObject
{
    public string settlementName;
    public SettlementSize size;
    public List<NPC> namedCitizens = new List<NPC>();
    [Space]
    public int playerReputation;
    public string[] localPlayerTitles;
    [Space]
    public float generalLocalCostMultiplier = 1f;
    public float localFoodCostMultiplier = 1f;
    public float localMaterialCostMultiplier = 1f;
    public float localLuxuryCostMultiplier = 1f;

    //public int crimeRate

    public string GetRandomTitle()
    {
        int i = localPlayerTitles.Length;
        if (i == 0) return "traveller";

        int num = Random.Range(0, i+1);
        return localPlayerTitles[num];
    }

    //Reset settlement values, citizen values, and local quest values
    public void ResetSettlement()
    {
        for (int i = 0; i < namedCitizens.Count; i++)
        {
            namedCitizens[i].ResetAllValues();
            for (int x = 0; x < namedCitizens[i].questsToGive.Length; x++)
            {
                namedCitizens[i].questsToGive[x].ResetQuest();
            }
        }
    }
}

public enum SettlementSize { Hamlet, Village, Town, City }
