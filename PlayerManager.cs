using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    #region - Singleton -

    public static PlayerManager instance;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    #endregion

    public delegate void OnCombatCallback(bool inCombat);
    public OnCombatCallback onCombatCallback;

    #region - Player Data -
    public string playerName = "Traveller";
    [HideInInspector] public string playerArchetype;
    [HideInInspector] public string playerClass;
    [HideInInspector] public string playerBackground;
    [HideInInspector] public string playerBirthplace;
    [HideInInspector] public int playerAge;
    [HideInInspector] public string playerVirtues;
    [HideInInspector] public string playerVices;
    [HideInInspector] public string playerHobbies;
    [HideInInspector] public Lifestyle playerLifestyle;

    public int playerLevel { get; private set; }
    public int totalExp { get; private set; }
    private int xpToNextLevel;

    //Player Location
    public string currentScene { get; private set; }
    public string playerLocation { get; private set; }
    public WorldRegion playerRegion;
    [Space]
    #endregion

    public GameObject player;
    public GameObject playerCenter;
    public Transform followTarget;

    [SerializeField] float deathDelay = 7f;
    public string sceneSpawnPoint = "defaultSP";

    private List<NPCController> enemyCombatants = new List<NPCController>();
    public bool playerInCombat { get; private set; }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerInCombat = false;

        CheckForPlayerEssentials();
    }

    #region - Scenes / Location -
    private void CheckForPlayerEssentials()
    {
        if (player == null)
            player = PlayerController.instance.gameObject;

        if (playerCenter == null)
            playerCenter = PlayerController.instance.center.transform.gameObject;

        if (followTarget == null)
            followTarget = PlayerController.instance.followTarget.transform;
    }

    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        CheckForPlayerEssentials();
        UIManager.instance.OnMenuClose();
        InputHandler.TogglePlayerInput_Static(true);
        PlayerController.instance.acceptInput = true;
        currentScene = s.name;

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("spawnPoint");
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.name == sceneSpawnPoint)
            {
                player.transform.position = spawnPoint.transform.position;
                break;
            }
            Debug.LogWarning("No spawnpoint found");
        }
        sceneSpawnPoint = "defaultSP";
    }

    public string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void OnPlayerLocationChange(string locationName)
    {
        playerLocation = locationName;
        //Have some sort of callback here for updates
    }
    #endregion

    #region - Player Death -
    public void KillPlayer()
    {
        StartCoroutine(ResetLevel());
    }

    private IEnumerator ResetLevel()
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        yield return new WaitForSeconds(deathDelay);
        stats.isDead = false;
        stats.FullRestore();
        Animator anim = player.GetComponentInChildren<Animator>();
        anim.Play("Locomotion", 0);
        anim.SetLayerWeight(1, 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region - Player Level -
    public void GainEXP(int xpValue)
    {
        totalExp += xpValue;
        if (totalExp >= xpToNextLevel)
        {
            OnPlayerLevelUp();
        }
        Debug.Log("Player gained +" + xpValue + " exp!");
    }

    private void OnPlayerLevelUp()
    {
        playerLevel++;
        Debug.Log("Player Leveled Up to Level: " + playerLevel);

        totalExp -= xpToNextLevel;
        CalculateXPToNextLevel();
    }

    private void CalculateXPToNextLevel()
    {
        //This is just a placeholder equation for now
        xpToNextLevel = Mathf.RoundToInt((playerLevel * 2) + 1) * 3;//Use this for testing
                                                                    //xPToNextLevel = ((baseValue * 2) + 1) * 30; //Use this for actual play
        if (totalExp >= xpToNextLevel) OnPlayerLevelUp();
    }
    #endregion

    public void OnCombatChange(NPCController controller, bool enteringCombat)
    {
        //When the player or one of their companions becomes the target of an enemy
        bool wasInCombat = playerInCombat;

        if (enteringCombat == true)
        {
            if (enemyCombatants.Contains(controller) == false) enemyCombatants.Add(controller);
        }
        else
        {
            if (enemyCombatants.Contains(controller) == true) enemyCombatants.Remove(controller);
        }

        if (enemyCombatants.Count > 0) playerInCombat = true;
        else playerInCombat = false;

        //Player either entered or left combat
        if (playerInCombat != wasInCombat)
        {
            if (onCombatCallback != null)
                onCombatCallback.Invoke(playerInCombat);
        }
    }

    #region - Saved Values -
    public void SetSavedValues(int level, int xp, string location)
    {
        playerLevel = level;
        totalExp = xp;
        CalculateXPToNextLevel();

        playerLocation = location;
    }
    public void SetSavedPlayerInfo(string name, string type, string pClass, string backG, string birth, int age, string virt, string vice, string hobbies, int life)
    {
        playerName = name;
        playerArchetype = type;
        playerClass = pClass;
        playerBackground = backG;
        playerBirthplace = birth;
        playerAge = age;
        playerVirtues = virt;
        playerVices = vice;
        playerHobbies = hobbies;
        playerLifestyle = (Lifestyle)life;
    }
    #endregion
}