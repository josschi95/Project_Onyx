using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Probably just store reference to this in the PlayerManager or something
[CreateAssetMenu(fileName = "Character Model Handler", menuName = "Character Model Handler")]
public class CharacterModelHandler : ScriptableObject
{
    public GameObject[] femaleHeads;
    public GameObject[] maleHeads;
    [Space]
    public GameObject[] hairModels;
    [Space]
    public GameObject[] maleBrows;
    public GameObject[] femaleBrows;
    [Space]
    public GameObject[] beardModels;
    [Space]
    public GameObject[] femaleBody;
    public GameObject[] maleBody;
}
