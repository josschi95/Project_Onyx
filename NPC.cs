using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "Characters/NPC")]
public class NPC : ScriptableObject
{
    public int npcID;
    [Tooltip("Name of character and the reference for the DialogueManager")]
    new public string name;
    public Lifestyle lifestyle;
    public SpokenLanguage primaryLanguage;
    [Tooltip("The character's faction association, if any")]
    public Faction faction;
    [Tooltip("Default location at which the NPC can be found, assuming no deviation")]
    public Settlement residence;
    [Tooltip("A measure of the character's disposition towards the player")]
    public int playerAffinity;
    [Tooltip("Used as a reference for checking quest states")]
    public bool isAlive = true;
    [Tooltip("Used as a reference for dialogue to give description on first interaction")]
    public bool hasMetPlayer = false;
    [Space]
    //For journal entries, I will likely end up making this a Serializable field
    //Have some sort of conditions for when each entry should be added
    //Low priority
    [Tooltip("Will appear in the player's journal as they interact with the character more")]
    public string[] journalEntries;
    [Tooltip("A list of all quests the character can give to the player")]
    public Quest[] questsToGive;
    [Tooltip("The character's known associates, excluding other faction members if relevant")]
    public NPC[] associates;
    [Tooltip("On first interaction, if the player has one of these backgrounds, +10 Aff")]
    public List<CharacterBackground> favoredBackgrounds = new List<CharacterBackground>();
    [Tooltip("On first interaction, if the player has one of these backgrounds, -10 Aff")]
    public List<CharacterBackground> hatedBackgrounds = new List<CharacterBackground>();
    [Space]
    public string ideals;
    public string flaws;
    public string bonds;
    public string goals;
    public string secrets;
    public string quirks;
    public string occupation;

    //Maybe some reference to their actual house/location they sleep/whatever

    public void ResetAllValues()
    {
        playerAffinity = 0;
        isAlive = true;
        hasMetPlayer = false;
    }

    public void OnCharacterDeath()
    {
        isAlive = false;
        for (int i = 0; i < questsToGive.Length; i++)
        {
            questsToGive[i].CheckForQuestProgression();
        }
    }
}
public enum Lifestyle { Squalid, Poor, Modest, Comfortable, Wealthy, Lavish }
public enum Faction { None, Paladins, Syndicate, Clan, Mages, Grove, Temple }
//Elementals and whatnot can all fall into primordial
public enum SpokenLanguage { Common, Demonic, Dwarvish, Elvish, Fey, Giant, Primordial, Hoarish, Beast }
