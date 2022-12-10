using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_CryptCall : MonoBehaviour
{
    public GameObject crypt;
    public GameObject skeleton;
    public int skeletonCount = 5;
    [SerializeField] GameObject FX1;
    [SerializeField] GameObject FX2;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnSpeed = 1f;

    void Start()
    {
        StartCoroutine(RaiseCrypt());
    }

    IEnumerator RaiseCrypt()
    {
        while (crypt.transform.position != transform.position)
        {
            crypt.transform.position = Vector3.MoveTowards(crypt.transform.position, transform.position, Time.deltaTime * spawnSpeed);
            yield return null;
        }
        FX1.SetActive(false);
        FX2.SetActive(false);
        StartCoroutine(SpawnSkeletons());
    }

    IEnumerator SpawnSkeletons()
    {
        for (int i = 0; i < skeletonCount; i++)
        {
            yield return new WaitForSeconds(0.2f);
            Instantiate(skeleton, spawnPoint.position, spawnPoint.transform.rotation);
        }
        StartCoroutine(LowerCrypt());
    }

    IEnumerator LowerCrypt()
    {
        FX1.SetActive(true);
        FX2.SetActive(true);

        Vector3 endPosition = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z);
        while (crypt.transform.position != endPosition)
        {
            crypt.transform.position = Vector3.MoveTowards(crypt.transform.position, endPosition, Time.deltaTime * spawnSpeed);
            yield return null;
        }
        Destroy(gameObject);
    }
}
