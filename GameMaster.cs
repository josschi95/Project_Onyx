using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public delegate void OnGamePaused();
    public OnGamePaused onGamePaused;

    public delegate void OnGameResumed();
    public OnGameResumed onGameResumed;

    //Singleton
    public static GameMaster instance;
    public JSONSaving saving;

    public int difficultyLevel { get; private set; }
    public int maxCombatTokens;
    public int currentCombatTokens;
    public bool gamePaused = false;    

    private void Awake()
    {
        Time.timeScale = 1;

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GameMaster found");
            Destroy(this);
            return;
        }
        else instance = this;

        if (difficultyLevel == 0) //Story difficulty
        {
            maxCombatTokens = 3;
        }
        else if (difficultyLevel == 1) //Normal difficulty
        {
            maxCombatTokens = 4;
        }
        else if (difficultyLevel == 2) //Hard difficulty
        {
            maxCombatTokens = 5;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        //Save Game if it's not the main menu nor character creation
        //if (s.buildIndex > 1) SaveGame();
    }

    public bool CheckForToken()
    {
        if (currentCombatTokens >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RetrieveToken()
    {
        currentCombatTokens--;
        if (currentCombatTokens < 0) currentCombatTokens = 0;
    }

    public void ReturnToken()
    {
        currentCombatTokens++;

        if (currentCombatTokens > maxCombatTokens)
        {
            currentCombatTokens = maxCombatTokens;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        gamePaused = true;

        if (onGamePaused != null) onGamePaused.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        gamePaused = false;

        if (onGameResumed != null) onGameResumed.Invoke();
    }

    public void SetDifficulty(int difficulty)
    {
        if (difficulty == 0)
        {
            difficultyLevel = 0;
            maxCombatTokens = 3;
        }
        else if (difficulty == 1)
        {
            difficultyLevel = 1;
            maxCombatTokens = 4;
        }
        else if (difficulty == 2)
        {
            difficultyLevel = 2;
            maxCombatTokens = 5;
        }
    }

    public void SaveGame()
    {
        saving.SaveData();
    }

    public void LoadGame()
    {
        saving.LoadData();
    }
}
