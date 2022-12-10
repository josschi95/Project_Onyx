using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class MainMenu : MonoBehaviour
{
    public Button continueButton, loadGameButton, newGameButton, settingsButton, controlsButton, quitButton;

    [Header("Camera")]
    public CinemachineVirtualCamera cam;
    public CinemachineDollyCart cart;

    public CinemachineSmoothPath[] paths;
    public Vector3[] cameraRotations;
    private int currentPath;

    private void Start()
    {
        Time.timeScale = 1;
        HUDManager.instance.HideImmediate();
        HUDManager.instance.hudLocked = true;

        newGameButton.onClick.AddListener(delegate 
        {
            HUDManager.instance.hudLocked = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
        });

        continueButton.onClick.AddListener(OnContinue);

        quitButton.onClick.AddListener(delegate 
        {
            Debug.Log("Quitting Game");
            Application.Quit(); 
        });

        currentPath = 0;
        cart.m_Path = paths[currentPath];
        cam.transform.localRotation = Quaternion.Euler(cameraRotations[currentPath]);

        UIManager.instance.ToggleCursor(true);
        InputHandler.TogglePlayerInput_Static(false);

        //CheckForExistingSave();
        if (!GameMaster.instance.saving.CheckForExistingSave())
        {
            continueButton.gameObject.SetActive(false);
            loadGameButton.gameObject.SetActive(false);
        }
    }

    private void CheckForExistingSave()
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath);
        if (saves.Length == 0)
        {

            Debug.Log("No Existing Save Files");
            return;
        }
        for (int i = 0; i < saves.Length; i++)
        {
            StreamReader reader = new StreamReader(saves[i]);
            string json = reader.ReadToEnd();

            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log(data.saveFileName);
        }
    }

    //The first file in the directory, need to make sure they're ordered by date/time though
    private void OnContinue()
    {
        HUDManager.instance.hudLocked = false;
        string[] files = Directory.GetFiles(Application.persistentDataPath);
        GameMaster.instance.saving.LoadFile(files[0]);
    }

    private void Update()
    {
        if (cart.m_Position == 1)
        {           
            currentPath++;
            if (currentPath >= paths.Length)
            {
                currentPath = 0;
            }
            cart.m_Path = paths[currentPath];
            cart.m_Position = 0;
            cam.transform.localRotation = Quaternion.Euler(cameraRotations[currentPath]);
            
        }
    }
}
