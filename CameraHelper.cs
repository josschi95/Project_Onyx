using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraHelper : MonoBehaviour
{
    #region - Singleton -
    public static CameraHelper instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("ERROR: More than one instance of CameraHelper found");
            return;
        }
        instance = this;
    }
    #endregion

    private UIManager UI;
    private PlayerCombat combat;
    private Player_Settings settings;
    [SerializeField] private CinemachineShake firstPersonShake;
    [SerializeField] private CinemachineShake followShake;
    [SerializeField] private CinemachineShake freeLookShake;
    public CameraView currentCamera; //{ get; private set; }

    public Camera mainCam;
    [HideInInspector] public CinemachineBrain brain;

    public bool moveRight = true;
    public bool aimCamActive = false;

    private float camZoom;
    private float zoomMultiplier = 0.01f;

    [Header("First Person")]
    public CinemachineVirtualCamera firstPerson_vcam;
    [SerializeField] private CinemachineInputProvider firstPersonInput;
    private CinemachinePOV pov;
    [SerializeField] private float min1PFov = 90f;
    [SerializeField] private float max1PFov = 120f;

    [Header("Third Person Follow")]
    public CinemachineVirtualCamera thirdPersonFollow_vcam;
    [SerializeField] private float min3PFollowFov = 30f;
    [SerializeField] private float max3PFollowFov = 90f;

    [Header("Alternate Cams")]
    public CinemachineVirtualCamera playerAimCam;
    public CinemachineVirtualCamera deathCam;

    private bool invertX;
    private bool invertY;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        combat = PlayerCombat.instance;
        settings = Player_Settings.instance;
        PlayerController.instance.onPlayerDialogueCallback += ToggleCameraInput;
        settings.onSettingsChanged += OnSettingsChanged;
        UI = UIManager.instance;
        brain = mainCam.GetComponent<CinemachineBrain>();
        pov = firstPerson_vcam.GetCinemachineComponent<CinemachinePOV>();
        CheckForPlayer();
        UpdateActiveCamera();
        OnSettingsChanged();
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        ResetCam();
        PlayerController.instance.turn.y = 0;
        PlayerController.instance.turn.x = PlayerController.instance.transform.rotation.y;
        CheckForPlayer();
    }

    private void OnSettingsChanged()
    {
        invertX = settings.invert_X;
        invertY = settings.invert_Y;

        //The default/"normal" settings for these is
        //Y inverted is normal, X not inverted is normal
        pov.m_HorizontalAxis.m_InvertInput = invertX;
        pov.m_VerticalAxis.m_InvertInput = !invertY;

        //Third person follow inver needs to be set through player controller

        //Still need to update sensitivity
    }

    private void CheckForPlayer()
    {
        //First Person
        if (firstPerson_vcam.m_Follow == null)
            firstPerson_vcam.m_Follow = PlayerController.instance.head;

        //Follow
        if (thirdPersonFollow_vcam.m_Follow == null)
            thirdPersonFollow_vcam.m_Follow = PlayerManager.instance.followTarget;
        if (thirdPersonFollow_vcam.m_LookAt == null)
            thirdPersonFollow_vcam.m_LookAt = PlayerManager.instance.followTarget;
    }

    //This still doesn't work how I want it to //Reference the portal script for what actually works
    public void ResetCam()
    {
        brain.enabled = false;
        brain.enabled = true;
        //vcam.enabled = false;
        //vcam.transform.position = vcam.m_Follow.position;
        //vcam.enabled = true;
    }

    public float FirstPersonRotation()
    {
        return pov.m_HorizontalAxis.Value;
    }

    //Method called through player Input
    public void CycleActiveCamera()
    {
        if (currentCamera == CameraView.First_Person) 
            currentCamera = CameraView.Follow;
        else currentCamera = CameraView.First_Person;
        UpdateActiveCamera();
    }

    public void UpdateActiveCamera()
    {
        if (currentCamera == CameraView.First_Person)
        {
            firstPerson_vcam.gameObject.SetActive(true);
            pov.m_RecenterTarget = CinemachinePOV.RecenterTargetMode.FollowTargetForward;
            thirdPersonFollow_vcam.gameObject.SetActive(false);

            camZoom = firstPerson_vcam.m_Lens.FieldOfView;
        }
        else
        {
            firstPerson_vcam.gameObject.SetActive(false);
            thirdPersonFollow_vcam.gameObject.SetActive(true);

            camZoom = thirdPersonFollow_vcam.m_Lens.FieldOfView;
        }
    }

    private void ToggleCameraInput(bool disable)
    {
        firstPersonInput.enabled = !disable;
    }

    public void CameraZoom(Vector2 value)
    {
        if (UI.dialogueMenuOpen || GameMaster.instance.gamePaused) return;

        camZoom -= value.y * zoomMultiplier;

        if (currentCamera == CameraView.First_Person)
        {
            camZoom = Mathf.Clamp(camZoom, min1PFov, max1PFov);
            firstPerson_vcam.m_Lens.FieldOfView = camZoom;
        }
        else
        {
            camZoom = Mathf.Clamp(camZoom, min3PFollowFov, max3PFollowFov);
            thirdPersonFollow_vcam.m_Lens.FieldOfView = camZoom;
        }
    }

    public IEnumerator AimZoomIn(float exitDelay)
    {
        yield return new WaitForSeconds(1.5f);
        playerAimCam.gameObject.SetActive(true);
        aimCamActive = true;
        while (combat.characterBowDrawn)
        {
            yield return null;
        }

        yield return new WaitForSeconds(exitDelay);
        playerAimCam.gameObject.SetActive(false);
        aimCamActive = false;
    }

    public void CameraShake(float intensity, float time)
    {
        if (currentCamera == CameraView.First_Person)
            firstPersonShake.ShakeCamera(intensity, time);
        else followShake.ShakeCamera(intensity, time);
    }

    void SwitchAimShoulder()
    {
        /*
        if (aimVirtualCamera.gameObject.activeSelf)
        {
            if (moveRight && vcam.CameraSide < 1)
            {
                vcam.CameraSide = Mathf.Lerp(vcam.CameraSide, 1, Time.deltaTime * 10f);
            }
            else if (!moveRight && vcam.CameraSide > 0)
            {
                vcam.CameraSide = Mathf.Lerp(vcam.CameraSide, 0, Time.deltaTime * 10f);
            }
        }
        */
    }
}

public enum CameraView { First_Person, Follow }

/* Zoom Settings
 * FOV: 30
 * Distance: 2
 */
