using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameControl : MonoBehaviour
{
    public ObjectPool Pool;
    public GenericProjectileSpell ProjectileSpell; 
    public TrailParticleControl TrailParticle;

    public List<GenericProjectileSpell> GenericProjectilePool = new List<GenericProjectileSpell>();
    public List<TrailParticleControl> TrailParticlePool = new List<TrailParticleControl>();

    public int NumberOfGenericProjectile = 50;
    public int NumberOfParticlesPerSpell = 20;

    // Start is called before the first frame update
    void Start()
    {
        // Generate the pool of Generic Projectiles
        GenericProjectilePool = Pool.setupInstancePool<GenericProjectileSpell>(
                                ProjectileSpell,
                                NumberOfGenericProjectile);

        // Create M particles for every Generic Projectiles
        for (int i = 0; i < NumberOfGenericProjectile; i ++)
        {
            TrailParticlePool.AddRange(Pool.setupInstancePool<TrailParticleControl>(
                            TrailParticle, 
                            NumberOfParticlesPerSpell));
        }
    }
}
