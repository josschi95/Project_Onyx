using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ActiveDivination : MonoBehaviour
{
    [SerializeField] private SphereCollider detectionArea;
    public DivinedObjects revealedTargets;
    public float area;
    public float duration;

    private void Start()
    {
        detectionArea.isTrigger = true;
        detectionArea.radius = area;
        StartCoroutine(DestroySelf(duration));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            var receiver = other.GetComponent<IEffectReceiver>();
            if (receiver != null)
                receiver.OnDivinationEnter(revealedTargets);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            var receiver = other.GetComponent<IEffectReceiver>();
            if (receiver != null)
                receiver.OnDivinationExit(revealedTargets);
        }
    }

    private IEnumerator DestroySelf(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}