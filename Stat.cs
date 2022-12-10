using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    public delegate void OnStatValueChanged(Stat stat, int magnitude);
    public OnStatValueChanged onStatValueChanged;

    public int Base_Value { get { return baseValue; } protected set { baseValue = value; } }
    [SerializeField] protected int baseValue = 10;
    public List<int> modifiers = new List<int>();
    [Space]
    [TextArea(3, 5)] public string statDescription;

    //Sets the base value of the stat to the given value
    public void SetBaseValue(int magnitude)
    {
        baseValue = magnitude;
    }

    //Returns the value of the stat with all current modifiers
    public int GetValue()
    {
        int finalValue = baseValue;
        for (int i = 0; i < modifiers.Count; i++)
        {
            finalValue += modifiers[i];
        }
        if (finalValue <= 0) finalValue = 0;
        return finalValue;
    }
    
    //Returns the value of the stat divided by 10 (0-10)
    public int GetStatDecim()
    {
        float value = baseValue;
        for (int i = 0; i < modifiers.Count; i++)
        {
            value += modifiers[i];
        }
        value = value * 0.1f;
        int finalValue = Mathf.FloorToInt(value);
        return finalValue;
    }

    //Permanently raises the base value of the stat
    public virtual void RaiseStat(int magnitude)
    {
        baseValue += magnitude;
        if (onStatValueChanged != null) 
            onStatValueChanged.Invoke(this, magnitude);
    }

    //Adds a modifier to the stat
    public void AddModifier(int magnitude)
    {
        if (magnitude != 0) modifiers.Add(magnitude);

        if (onStatValueChanged != null)
            onStatValueChanged.Invoke(this, magnitude);
    }

    //Removes existing modifier to the stat
    public void RemoveModifier(int magnitude)
    {
        if (magnitude != 0) modifiers.Remove(magnitude);

        if (onStatValueChanged != null)
            onStatValueChanged.Invoke(this, magnitude);
    }
}