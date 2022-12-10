using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectPlacement : MonoBehaviour
{
    public ObjectPlacementHelper helper;
    public GameObject objectToSpawn;
    public bool useArray = false;

    public int amount = 10;

    public float objectXSpread = 10;
    public float objectYSpread = 0;
    public float objectZSpread = 10;
    [Space]
    public float objectMinScale = 0.5f;
    public float objectMaxScale = 2;

    [ContextMenu("Spawn Objects")]
    void Spawn()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-objectXSpread, objectXSpread), Random.Range(-objectYSpread, objectYSpread), Random.Range(-objectZSpread, objectZSpread)) + transform.position;
            GameObject go = null;

            if (useArray == false)
            {
                go = Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
            }
            else
            {
                int num = Random.Range(0, helper.gameObjects.Length);
                go = Instantiate(helper.gameObjects[num], randomPosition, Quaternion.identity);
            }
            go.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            float scale = Random.Range(objectMinScale, objectMaxScale);
            go.transform.localScale = new Vector3(scale, scale, scale);

            go.transform.SetParent(gameObject.transform);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(objectXSpread, objectYSpread, objectZSpread));
    }
}
