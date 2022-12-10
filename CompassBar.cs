using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CompassBar : MonoBehaviour
{
    private Player_Settings settings;
    private bool useQuestMarker;
    [SerializeField] private GameObject questMarkerPrefab;
    [SerializeField] private QuestMarker questMarker;

    public RectTransform compassBar;
    [Space]
    public RectTransform northMarkerRect;
    public RectTransform southMarkerRect;
    public RectTransform questMarkerRect;
    [Space]
    private Transform cameraTransform;
    public Transform questMarkerTransform;

    private Coroutine toggleCoroutine;
    private Vector2 shownPos;
    private Vector2 hiddenPos;

    private void Start()
    {
        settings = Player_Settings.instance;
        cameraTransform = Camera.main.transform;
        settings.onSettingsChanged += ToggleQuestMarker;

        if (questMarker == null) questMarker = QuestMarker.instance;
        if (questMarker == null) GetNewQuestMarker();
        questMarkerTransform = questMarker.transform;
        questMarker.onQuestMarkerUpdate += ToggleQuestMarker;
        ToggleQuestMarker();

        shownPos = compassBar.anchoredPosition;
        hiddenPos = shownPos;
        hiddenPos.y += 200;
    }

    void Update()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        SetMarkerPosition(northMarkerRect, Vector3.forward * 1000);
        SetMarkerPosition(southMarkerRect, Vector3.back * 1000);

        if (useQuestMarker && questMarker.hasPosition)
            SetMarkerPosition(questMarkerRect, questMarkerTransform.position);
    }

    private void SetMarkerPosition(RectTransform markerTransform, Vector3 worldPosition)
    {
        Vector3 directionToTarget = worldPosition - cameraTransform.position;
        float angle = Vector2.SignedAngle(new Vector2(directionToTarget.x, directionToTarget.z), new Vector2(cameraTransform.transform.forward.x, cameraTransform.transform.forward.z));
        float compassPositionX = Mathf.Clamp(2 * angle / Camera.main.fieldOfView, -1, 1);
        markerTransform.anchoredPosition = new Vector2(compassBar.rect.width / 2 * compassPositionX, 0);
    }

    //Toggle the quest marker icon on or off if there is an active quest
    public void ToggleQuestMarker()
    {
        useQuestMarker = settings.displayQuestMarker;
        if (useQuestMarker && questMarker.hasPosition)
        {
            questMarkerRect.gameObject.SetActive(true);
        }
        else
        {
            questMarkerRect.gameObject.SetActive(false);
        }
    }

    private void GetNewQuestMarker()
    {
        GameObject qm = Instantiate(questMarkerPrefab);
        questMarker = qm.GetComponent<QuestMarker>();
    }

    public void ToggleCompassBar(bool hide)
    {
        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);
        toggleCoroutine = StartCoroutine(ToggleCompassCoroutine(hide));
    }

    public void HideImmediate()
    {
        if (toggleCoroutine != null) StopCoroutine(toggleCoroutine);
        compassBar.anchoredPosition = hiddenPos;
    }

    private float timeToMove = 0.25f;
    private IEnumerator ToggleCompassCoroutine(bool hide)
    {
        float elapsedTime = 0;
        var statStart = compassBar.anchoredPosition;
        var statEnd = shownPos;

        if (hide == true)
        {
            statEnd = hiddenPos;
        }

        while (elapsedTime < timeToMove)
        {
            compassBar.anchoredPosition = Vector2.Lerp(statStart, statEnd, (elapsedTime / timeToMove));
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        compassBar.anchoredPosition = statEnd;
    }
}
