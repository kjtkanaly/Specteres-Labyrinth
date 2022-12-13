using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> PooledObjects = new List<GameObject>();
    public GameObject ObjectToPool = null;
    public int PoolSize = 0;

    // Start is called before the first frame update
    public void Start()
    {   
        // Checks
        if (ObjectToPool == null)
        {
            Debug.LogError("An object to pool is needed!");
            return;
        }

        setupInstancePool();
    }


    // Setup up pool
    public void setupInstancePool()
    {
        PooledObjects = new List<GameObject>();

        for (int i = 0; i < PoolSize; i++)
        {
            GameObject ObjectInstance = Instantiate(ObjectToPool.gameObject);
            ObjectInstance.SetActive(false);
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
