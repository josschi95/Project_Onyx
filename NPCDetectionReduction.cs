using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDetectionReduction : MonoBehaviour
{
    public float reducedVisionRadius = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            var detect = other.GetComponentInParent<AICharacterDetection>();
            if (detect != null)
            {
                detect.OnDetectionRadiusChange(reducedVisionRadius);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger == false)
        {
            var detect = other.GetComponentInParent<AICharacterDetection>();
            if (detect != null)
            {
                detect.OnDetectionRadiusChange(detect.maximumDetectionRadius);
            }
        }
    }
}
