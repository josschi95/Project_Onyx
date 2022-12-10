using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    [SerializeField] private NPCController[] sceneNPC;
    [SerializeField] private GameObject[] NPCNodes;

    void Start()
    {
        StartCoroutine(SetupDelay());
    }

    //Places NPCs in starting positions
    private void PlaceNPCs()
    {
        for (int i = 0; i < sceneNPC.Length; i++)
        {
            var node = NPCNodes[i].GetComponent<INPCInteractNode>();
            if (node != null)
            {
                sceneNPC[i].StopAgent(true);
                sceneNPC[i].agent.enabled = false;
                node.Interact(sceneNPC[i]);
            }
            else Debug.LogError("No INPCInteractNode Found. Did you assign an incorrect object?");
        }
    }

    private IEnumerator SetupDelay()
    {
        yield return new WaitForSeconds(0.1f);
        PlaceNPCs();
    }
}
