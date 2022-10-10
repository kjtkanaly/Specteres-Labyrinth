using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling SharedInstance;

    public List<GameObject> PooledObjects;
    public GameObject ObjectToPool;

    public List<GenericProjectileSpell> PooledProjectileSpell;
    public GenericProjectileSpell SpellToPool;

    public int AmountToPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        PooledObjects = new List<GameObject>();
        GameObject tmp;

        for (int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(ObjectToPool);
            tmp.SetActive(false);
            PooledObjects.Add(tmp);
        }

        for (int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(SpellToPool.gameObject);
            tmp.SetActive(false);
            PooledProjectileSpell.Add(tmp.GetComponent<GenericProjectileSpell>());
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < AmountToPool; i++)
        {
            if (!PooledObjects[i].activeInHierarchy)
            {
                return PooledObjects[i];
            }
        }

        return null;
    }

    public GenericProjectileSpell GetPooledProjectileSpell()
    {
        for (int i = 0; i < AmountToPool; i++)
        {
            if (!PooledProjectileSpell[i].gameObject.activeInHierarchy)
            {
                return PooledProjectileSpell[i];
            }
        }

        return null;
    }

}
