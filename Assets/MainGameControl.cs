using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameControl : MonoBehaviour
{
    public ObjectPool Pool;
    public GenericProjectileSpell ProjectileSpell; 

    public List<GenericProjectileSpell> GenericProjectilePool = new List<GenericProjectileSpell>();

    public int NumberOfGenericProjectile = 100;

    // Start is called before the first frame update
    void Start()
    {
        GenericProjectilePool = Pool.setupInstancePool<GenericProjectileSpell>(ProjectileSpell,
                                                                               NumberOfGenericProjectile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
