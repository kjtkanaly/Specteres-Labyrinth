using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectileSpell : Spell
{
    public Rigidbody2D RB;
    public Transform Trans;
    public SpriteRenderer Spr;
    public CircleCollider2D Col;
    public MainGameControl MainGameCtrl;
    public ObjectPool ObjectPool;
    public BoxCollider2D[]  PlayerColliders;

    private void Awake()
    {
        RB = this.GetComponent<Rigidbody2D>();
        Trans = this.GetComponent<Transform>();        
        Spr = this.GetComponentInChildren(typeof(SpriteRenderer), true) as SpriteRenderer;
        Col = this.GetComponent<CircleCollider2D>();
        MainGameCtrl = GameObject.FindGameObjectWithTag("Main Game").GetComponent<MainGameControl>();
        ObjectPool = GameObject.FindGameObjectWithTag("Main Game").GetComponent<ObjectPool>();
        PlayerColliders = GameObject.FindGameObjectWithTag("Player").GetComponents<BoxCollider2D>();
    }


    private void OnEnable()
    {
        foreach (BoxCollider2D playerCol in PlayerColliders)
        {
            Physics2D.IgnoreCollision(Col, playerCol);
        }

        StartCoroutine(ParticleTimer());
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        /* if (col.collider.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(col.collider, col.otherCollider);
        } */

        if (CanBounce == false)
        {
            this.gameObject.SetActive(false);
        }
    }


    private void OnDisable()
    {
        this.Name = "";
        this.ManaDrain = 0;

        this.Speed      = 0f;
        this.Damage     = 0f;
        this.CastDelay  = 0f;
        this.Lifetime   = 0f;
        this.Spread     = 0f;
    }


    public IEnumerator ParticleTimer()
    {
        while(this.gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(particleTimeDelay);
            TrailParticleControl TrailParticle = 
            ObjectPool.GetObjectFromThePool<TrailParticleControl>(MainGameCtrl.TrailParticlePool);

            if (TrailParticle != null)
            {
                TrailParticle.particleColor = particleColor;

                // Activate the spell
                TrailParticle.gameObject.SetActive(true);

                // Setting the spell's spawn location
                TrailParticle.transform.SetParent(this.transform);
                TrailParticle.transform.localPosition = new Vector3(0f, 0f);
                TrailParticle.transform.SetParent(null);

                // Set the particle to be a percentage of the projecitle's velocity
                TrailParticle.RB.velocity = new Vector2((RB.velocity.x * 
                                                        particleSpeedPerecent), 
                                                        0f);

                TrailParticle.StartCoroutine(TrailParticle.FadeTimer());
            }
        }
    }

    public IEnumerator LifetimeTimer()
    {
        yield return new WaitForSeconds(Lifetime);
        StopCoroutine(ParticleTimer());
        this.gameObject.SetActive(false);
    }
}

// This script is assinged to our 'Generic' Projectile objects. 
// The generic projectile is what a wand will actually launch.