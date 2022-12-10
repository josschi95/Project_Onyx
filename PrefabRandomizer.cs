using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabRandomizer : MonoBehaviour
{
    public GameObject[] itemsToRandomize;
    public GameObject[] replacementPrefabs;

    [ContextMenu("Randomize")]
    void RandomizeItems()
    {
        foreach (GameObject go in itemsToRandomize)
        {
            int num = Random.Range(0, replacementPrefabs.Length);
            GameObject newSpawn = Instantiate(replacementPrefabs[num], go.transform.localPosition, go.transform.localRotation, go.transform.parent);
        }
    }
}
