using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestMarker : MonoBehaviour
{
    #region - Singleton -
    public static QuestMarker instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("ERROR: More than one QuestMarker in scene");
            Destroy(this);
            return;
        }
        instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(this);
    }
    #endregion

    public delegate void OnQuestMarkerUpdate();
    public OnQuestMarkerUpdate onQuestMarkerUpdate;

    private QuestManager manager;
    private Quest trackedQuest;
    [HideInInspector] public bool hasPosition;

    private void Start()
    {
        manager = QuestManager.instance;
        manager.onTrackedQuestChanged += UpdateTrackedQuest;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //Function called to update the quest marker when the current scene changes, funnels into UpdateTrackedQuest
    private void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        UpdateTrackedQuest(trackedQuest);
    }

    public void ResetMarker()
    {
        if (trackedQuest != null)
        {
            trackedQuest.onQuestAdvanced -= delegate { UpdateQuestMarkerPosition(); };
            trackedQuest = null;
        }
        transform.SetParent(null);
        transform.position = Vector3.zero;
        hasPosition = false;
        DontDestroyOnLoad(gameObject);
    }

    //Function called to update the quest marker when the currently tracked quest changes
    private void UpdateTrackedQuest(Quest quest)
    {
        ResetMarker();

        if (quest != null)
        {
            trackedQuest = quest;
            trackedQuest.onQuestAdvanced += delegate { UpdateQuestMarkerPosition(); };
            UpdateQuestMarkerPosition();
        }

        if (onQuestMarkerUpdate != null) onQuestMarkerUpdate.Invoke();
    }

    //Function to actually change the position of the quest marker
    private void UpdateQuestMarkerPosition()
    {
        if (trackedQuest != null)
        {
            //The current state was not given a quest marker, don't set position
            if (trackedQuest.currentState.questMarkerName == "") return;

            GameObject[] gos = GameObject.FindGameObjectsWithTag("questMarker");
            for (int i = 0; i < gos.Length; i++)
            {
                if (gos[i].name == trackedQuest.currentState.questMarkerName)
                {
                    transform.SetParent(gos[i].transform, false);
                    transform.localPosition = Vector3.zero;
                    hasPosition = true;
                    break;
                }
            }
        
            /*
            string targetScene = trackedQuest.currentState.questMarkerSceneName;
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == targetScene)
            {
                SetMarker();
            }
            else
            {
                SetMarkerOnScenePortal();
            }
            */
        }
    }
}
