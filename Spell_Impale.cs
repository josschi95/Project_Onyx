using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Impale : MonoBehaviour
{
    public float impaleDelay = 1f;
    [SerializeField] GameObject[] spikes;

    // Start is called before the first frame update
    void Start()
    {
        //Probably add some damage script onto each spike
        StartCoroutine(Impale());
    }

    IEnumerator Impale()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < spikes.Length; i++)
        {
            yield return new WaitForSeconds(impaleDelay);
            spikes[i].transform.position = Vector3.MoveTowards(spikes[i].transform.position, transform.position, 3f);
        }

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
