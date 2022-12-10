using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonSpawn : MonoBehaviour
{
    public GameObject skeleton;
    [SerializeField] float spawnSpeed = 1f;
    private NavMeshAgent agent;
    

    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
        GameObject newSkeleton = Instantiate(skeleton, spawnPosition, transform.rotation);
        agent = newSkeleton.GetComponent<NavMeshAgent>();
        agent.enabled = false;
        StartCoroutine(RaiseSkeleton(newSkeleton));
    }

    IEnumerator RaiseSkeleton(GameObject newSkeleton)
    {
        while (newSkeleton.transform.position != transform.position)
        {
            newSkeleton.transform.position = Vector3.MoveTowards(newSkeleton.transform.position, transform.position, Time.deltaTime * spawnSpeed);
            yield return null;
        }
        agent.enabled = true;
        Destroy(gameObject);
    }
}