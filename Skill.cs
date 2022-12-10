using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill : Stat
{
    public int currentXP;// { get; private set; }
    public int xPToNextLevel;// { get; private set; }

    public override void RaiseStat(int magnitude)
    {
        if (baseValue == 100) return;
        if (baseValue + magnitude > 100)
            magnitude = 100 - baseValue;

        baseValue += magnitude;

        if (onStatValueChanged != null)
            onStatValueChanged.Invoke(this, magnitude);
    }

    public void GrantStatXP(int xp)
    {
        if (baseValue == 100) return;

        currentXP += xp;
        if (currentXP >= xPToNextLevel)
        {
            RaiseStat(1);
            currentXP -= xPToNextLevel;
            CalculateXPToNextLevel();
        }
    }

    private void CalculateXPToNextLevel()
    {
        xPToNextLevel = Mathf.RoundToInt((baseValue * 2) + 1) * 3;//Use this for testing
        //xPToNextLevel = ((baseValue * 2) + 1) * 30; //Use this for actual play
        
        //Just in case the player gets a massive xp gain, check again for level up
        if (currentXP >= xPToNextLevel)
        {
            RaiseStat(1);
            currentXP -= xPToNextLevel;
            CalculateXPToNextLevel();
        }
    }

    public void SetSavedValue(int level, int xp)
    {
        baseValue = level;
        currentXP = xp;
        CalculateXPToNextLevel();
        if (onStatValueChanged != null)
            onStatValueChanged.Invoke(this, level);
    }
}
