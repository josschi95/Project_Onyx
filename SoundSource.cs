using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SoundSource : MonoBehaviour
{
    public SphereCollider soundCollider;

    public float soundMultiplier = 1f;

    // Start is called before the first frame update
    public virtual void Start()
    {
        soundCollider = GetComponent<SphereCollider>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            NPCController enemy = other.GetComponent<NPCController>();
            if (enemy != null) enemy.OnSoundDetected(transform.position);
        }
    }
}