using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Where would scene portals be used?
 * Doors
 * Cave/Dungeon Entrances
 * Basically entrances and exits
 * 
 * So when would they not be able to be accessed?
 * If the door was locked
 * so maybe a subclass of this for ScenePortal_Door?
*/

public class ScenePortal_Interactable : MonoBehaviour, IInteractable
{
    [Header("SceneManager Info")]
    [Tooltip("The name of the scene that will be loaded")]
    public string sceneToLoad;
    [Tooltip("The name of the spawnPoint at which the player will start in next scene")]
    public string nextSceneSpawnPoint;
    [Tooltip("All scenes which can only be accessed through the next scene")]
    public string[] subScenes;

    [Header("Interactable")]
    public string interactionMethod = "Enter";
    public string sceneDisplayName;
    public float interactionDistance = 2;

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        //Where would scene portals be used? On Doors I think?
        return true;
    }

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString(interactionMethod, sceneDisplayName, false);
    }

    public void Interact(CharacterController controller)
    {
        if (controller is PlayerController)
        {
            PlayerManager.instance.sceneSpawnPoint = nextSceneSpawnPoint;
            QuestMarker.instance.ResetMarker();
            if (nextSceneSpawnPoint == "")
            {
                Debug.LogWarning("ERROR: No SpawnPoint assigned");
            }
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
