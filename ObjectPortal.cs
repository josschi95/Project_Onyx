using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPortal : MonoBehaviour
{
    [SerializeField] ObjectPortal brotherPortal;
    public Transform outPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.isStatic == false)
        {
            other.transform.position = brotherPortal.outPosition.position;
        }        
    }
}
