using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPooling : MonoBehaviour
{
    public static SpellPooling SharedInstance;

    public List<GenericProjectileSpell> PooledSpell;
    public GenericProjectileSpell SpellToPool;

    public int AmountToPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        PooledSpell = new List<GenericProjectileSpell>();
        GameObject tmp;

        for (int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(SpellToPool.gameObject);
            tmp.SetActive(false);
            PooledSpell.Add(tmp.GetComponent<GenericProjectileSpell>());
        }
    }

    public GenericProjectileSpell GetPooledSpell()
    {
        for (int i = 0; i < AmountToPool; i++)
        {
            if (!PooledSpell[i].gameObject.activeInHierarchy)
            {
                return PooledSpell[i];
            }
        }

        return null;
    }

}
