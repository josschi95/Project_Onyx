using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Team", menuName = "Characters/Team")]
public class Team : ScriptableObject
{
    new public string name;
    public List<Team> alliedTeams = new List<Team>();
}
