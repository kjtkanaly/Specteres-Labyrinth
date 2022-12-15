using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour 
{
    public List<T> setupInstancePool<T>(T objectToPool, int PoolSize = 1) where T : MonoBehaviour
    {
        List<T> PooledObjects = new List<T>();

        for (int i = 0; i < PoolSize; i++)
        {
            T ObjectInstance = Instantiate(objectToPool);
            ObjectInstance.gameObject.SetActive(false);
            PooledObjects.Add(ObjectInstance);
        }

        return PooledObjects;
    }
    
    // Grab an object from the pool
    public T GetObjectFromThePool<T>(List<T> Pool) where T : MonoBehaviour
    {
        for (int i = 0; i < Pool.Count; i++)
        {
            if (!Pool[i].gameObject.activeInHierarchy)
            {
                return Pool[i];
            }
        }

        return null;
    }
}
