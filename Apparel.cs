using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Apparel", menuName = "Inventory/Apparel/Apparel")]
public class Apparel : Equipment
{
    [Header("Apparel Properties")]
    public int armor;
    [Space]
    public bool glovesWithForearms = false;
    public ArmorSkill armorSkill;
    public GameObject[] assignedMeshes;
    public GameObject[] newApparelObjects;
}

public enum ArmorSkill { Unarmored, Light, Medium, Heavy }