using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour 
{
    // Setup up pool
    public void setupInstancePool<T>(int PoolSize, T objectToPool) where T : new()
    {
        List<T> PooledObjects = new List<T>();

        for (int i = 0; i < PoolSize; i++)
        {
            ObjectInstance = Instantiate(objectToPool);
            ObjectInstance.gameObject.SetActive(false);
            PooledObjects.Add(ObjectInstance);
        }
    }


    // Grab an object from the pool
    public GameObject GetObjectFromThePool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            if (!PooledObjects[i].activeInHierarchy)
            {
                return PooledObjects[i];
            }
        }

        return null;
    }
}
