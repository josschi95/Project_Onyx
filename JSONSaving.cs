using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JSONSaving : MonoBehaviour
{
    private PlayerData playerData;
    [SerializeField] private DatabaseManager databaseManager;

    private string path = "";
    private string persistentPath = "";

    private void Start()
    {
        SetPaths();
    }

    //Checks the Persistent Data path for all files within the location
    public bool CheckForExistingSave()
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath);
        if (saves.Length == 0)
        {
            Debug.Log("No Existing Save Files");
            return false;
        }

        for (int i = 0; i < saves.Length; i++)
        {
            //This works so long as the file name contains SaveData
            if (!saves[i].Contains("SaveData")) continue;
            StreamReader reader = new StreamReader(saves[i]);
            string json = reader.ReadToEnd();

            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log(data.saveFileName);
        }
        return true;
    }

    private void CreatePlayerData()
    {
        playerData = new PlayerData(PlayerStats.instance, PlayerSpellcasting.instance, PlayerInventory.instance);
    }

    private void SetPaths()
    {
        //This one isn't even in use
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json";

        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
    }

    public void SaveData()
    {
        string savePath = persistentPath;
        //Debug.Log("Saving Data at " + savePath);

        CreatePlayerData();
        string json = JsonUtility.ToJson(playerData, true);
        Debug.Log(json);

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
    }

    public void LoadData()
    {
        StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();

        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        //Debug.Log("this isn't doing anything yet");
        SetPlayerValues(data);
    }

    public void LoadFile(string fileName)
    {
        StreamReader reader = new StreamReader(fileName);
        string json = reader.ReadToEnd();

        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        //Debug.Log("this isn't doing anything yet");
        SetPlayerValues(data);
    }

    private void SetPlayerModel(PlayerData data)
    {
        Color skin = new Color(data.skinColor[0], data.skinColor[1], data.skinColor[2]);
        Color hair = new Color(data.hairColor[0], data.hairColor[1], data.hairColor[2]);
        Color stub = new Color(data.stubbleColor[0], data.stubbleColor[1], data.stubbleColor[2]);
        Color scar = new Color(data.scarColor[0], data.scarColor[1], data.scarColor[2]);
        Color eyes = new Color(data.eyeColor[0], data.eyeColor[1], data.eyeColor[2]);
        Color paint = new Color(data.paintColor[0], data.paintColor[1], data.paintColor[2]);
        PlayerEquipmentManager.instance.SetPlayerModel(data.isMale, skin, hair, stub, scar, eyes, paint, data.facialModels);
    }

    private void SetPlayerInventory(PlayerData data)
    {
        var inventory = PlayerInventory.instance;
        inventory.ClearInventory();
        inventory.AddCurrency(CurrencyType.Copper, data.copperPieces);
        inventory.AddCurrency(CurrencyType.Silver, data.silverPieces);
        inventory.AddCurrency(CurrencyType.Gold, data.goldPieces);
        databaseManager.itemDatabase.AssignIDs();

        for (int i = 0; i < data.allItems.Count; i++)
        {
            Item tempItem = databaseManager.itemDatabase.items[data.allItems[i].itemID];
            //Item newItem = itemDatabase.GetItem[data.allItems[i].itemID];
            inventory.AddItem(tempItem, data.allItems[i].quantity);
        }

        inventory.cookBook.Clear();
        databaseManager.recipeDatabase.AssignIDs();
        for (int i = 0; i < data.allRecipes.Count; i++)
        {
            Recipe newRecipe = databaseManager.recipeDatabase.GetRecipe[data.allRecipes[i].recipeID];
            inventory.LearnRecipe(newRecipe);
        }

        inventory.abilities.Clear();
        databaseManager.abilityDatabase.AssignIDs();
        for (int i = 0; i < data.allAbilities.Count; i++)
        {
            Ability newAbility = databaseManager.abilityDatabase.GetAbility[data.allAbilities[i].abilityID];
            inventory.LearnAbility(newAbility);
        }

        PlayerEquipmentManager.instance.UnequipAll();
        for (int i = 0; i < data.currentEquip.Length; i++)
        {
            //I don't know why but I expect to run into issues here.
            if (data.currentEquip[i] >= 0)
            {
                var item = databaseManager.itemDatabase.items[data.currentEquip[i]] as Equipment;
                if (item is Weapon weapon)
                {
                    for (int x = 0; x < inventory.weapons.Count; x++)
                    {
                        if (DatabaseManager.GetItem(inventory.weapons[x].itemID) == item)
                        //if (inventory.weapons[x].item == item)
                        {
                            //Left Hand
                            if (i == 1 || i == 3) inventory.weapons[x].UseSecondary(PlayerController.instance);
                            //Right Hand
                            else inventory.weapons[x].Use(PlayerController.instance);
                        }
                    }
                }
                else if (item is Apparel apparel)
                {
                    for (int x = 0; x < inventory.apparel.Count; x++)
                    {
                        if (DatabaseManager.GetItem(inventory.apparel[x].itemID) == item)
                        //if (inventory.apparel[x].item == item)
                        {
                            inventory.apparel[x].Use(PlayerController.instance);
                        }
                    }
                }
            }
        }
        PlayerEquipmentManager.instance.SwapWeaponSets(data.usingSecondary, false);
    }

    private void SetPlayerQuests(PlayerData data)
    {
        //I should probably initialize this last just in case the status of any item/NPC would affect the quest

        //I will likely have to go through and serialize each quest stage that has been completed
        //and re-mark them as complete/failed or whatever other outcome I have set
        Quest current = null;
        if (data.curreneQuest >= 0) current = databaseManager.questDatabase.quests[data.curreneQuest];

        var active = new List<Quest>();
        for (int i = 0; i < data.activeQuests.Count; i++)
        {
            active.Add(databaseManager.questDatabase.quests[data.activeQuests[i]]);
        }
        var complete = new List<Quest>();
        for (int i = 0; i < data.completedQuests.Count; i++)
        {
            complete.Add(databaseManager.questDatabase.quests[data.completedQuests[i]]);
        }
        var failed = new List<Quest>();
        for (int i = 0; i < data.failedQuests.Count; i++)
        {
            failed.Add(databaseManager.questDatabase.quests[data.failedQuests[i]]);
        }
        var abandoned = new List<Quest>();
        for (int i = 0; i < data.abandonedQuests.Count; i++)
        {
            abandoned.Add(databaseManager.questDatabase.quests[data.abandonedQuests[i]]);
        }

        QuestManager.instance.SetSavedValues(current, active, data.activeQuestStages, complete, failed, abandoned);
    }

    private void SetPlayerSpellcasting(PlayerData data)
    {
        PlayerSpellcasting casting = PlayerSpellcasting.instance;
        casting.casterDomain = (MagicalDomain)data.casterDomain;
        casting.casterCovenant = (Deities)data.playerCovenant;
        casting.divineDevotion = data.playerDevotion;

        casting.arcaneSpellbook.Clear();
        databaseManager.spellDatabase.AssignIDs();
        for (int i = 0; i < data.allSpells.Count; i++)
        {
            Spell newSpell = databaseManager.spellDatabase.GetSpell[data.allSpells[i].spellID];
            casting.LearnSpell(newSpell);
        }
    }

    private void SetPlayerValues(PlayerData data)
    {
        Debug.Log("Setting Player Values");

        //Loads the saved scene and place the player
        GameMaster.instance.SetDifficulty(data.difficulty);
        SceneManager.LoadScene(data.currentScene);
        DateTimeManager.instance.SetSavedDateTime(data.timeOfDay, data.dayOfWeek, data.dayOfMonth, data.currentMonth, data.year, data.totalDayCount);
        PlayerManager.instance.player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        PlayerManager.instance.player.transform.rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);

        //Player Stats and Skills
        PlayerManager.instance.SetSavedValues(data.playerLevel, data.playerXP, data.playerLocation);
        PlayerManager.instance.SetSavedPlayerInfo(data.pName, data.pArch, data.pClass, data.pBack, data.pBirth, data.age, data.pVirt, data.pVice, data.pHobby, data.pLife);

        PlayerStats player = PlayerStats.instance;
        player.SetSavedValues(data.currentHealth, data.currentMana, data.currentStamina);
        player.statSheet.SetSavedValues(data); //Just pass the whole thing, there's a lot to unpack
        //Current Stat Modifiers
        for (int i = 0; i < data.activeEffects.Count; i++)
        {
            player.AddSpellEffect(data.activeEffects[i].effect, data.activeEffects[i].statIndex,
                data.activeEffects[i].magnitude, data.activeEffects[i].duration, data.activeEffects[i].sourceName);
        }

        SetPlayerModel(data);

        SetPlayerSpellcasting(data);

        SetPlayerInventory(data);

        SetPlayerQuests(data);

        //Journal (Quests, NPCs, Bestiary)
    }
}
