using UnityEngine;

public interface IPooledObject
{
    //Use this in place of the start function for objects pulled from the pool
    void OnObjectSpawn();
}
