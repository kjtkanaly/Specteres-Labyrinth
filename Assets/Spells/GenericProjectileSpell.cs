using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectileSpell : Spell
{
    // RB: Object's RigidBody
    // Trans: Object's Transform
    public Rigidbody2D      RB;
    public Transform        Trans;
    public Transform        PlayerTrans;
    public SpriteRenderer   Spr;
    public CircleCollider2D Col;
    public BoxCollider2D[]  PlayerColliders;

    private void Awake()
    {
        RB = this.GetComponent<Rigidbody2D>();
        Trans = this.GetComponent<Transform>();        
        Spr = this.GetComponentInChildren(typeof(SpriteRenderer), true) as SpriteRenderer;
        Col = this.GetComponent<CircleCollider2D>();
        PlayerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerColliders = GameObject.FindGameObjectWithTag("Player").GetComponents<BoxCollider2D>();
    }


    private void OnEnable()
    {
        Debug.Log(PlayerColliders.Length);
        foreach (BoxCollider2D playerCol in PlayerColliders)
        {
            Physics2D.IgnoreCollision(Col, playerCol);
        }
        /*
        // Setting the spell's spawn location
        this.transform.SetParent(PlayerTrans.transform.GetChild(0).transform);
        this.transform.localPosition = new Vector3(0f, 0.63f);
        this.transform.SetParent(null);

        // Get the Spell's Generic Spell Class
        GenericProjectileSpell GenericSpell = GenericSpellObj.GetComponent<GenericProjectileSpell>();

        // Setting the spell's velocity
        GenericSpell.RB.velocity = WandDirection.MousePos.normalized * CurrentProjectile.Speed;

        // Setting the spell's parameters
        GenericSpell.Name       = CurrentProjectile.Name;
        GenericSpell.ManaDrain  = CurrentProjectile.ManaDrain;
        GenericSpell.Speed      = CurrentProjectile.Speed;
        GenericSpell.Damage     = CurrentProjectile.Damage;
        GenericSpell.CastDelay  = CurrentProjectile.CastDelay;
        GenericSpell.Lifetime   = CurrentProjectile.Lifetime;
        GenericSpell.Spread     = CurrentProjectile.Spread;
        GenericSpell.CanBounce  = CurrentProjectile.CanBounce;

        // Setting the spell's physic
        GenericSpell.RB.sharedMaterial.bounciness = CurrentProjectile.Bounce;
        GenericSpell.RB.sharedMaterial.friction = CurrentProjectile.Friction;

        // Setting the projectile's sprite
        GenericSpell.Spr.sprite = CurrentProjectile.SpriteImage;

        // Starting the lifetime timer
        StartCoroutine(GenericSpell.LifetimeTimer());
        */
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
        this.ManaDrain = 0f;

        this.Speed      = 0f;
        this.Damage     = 0f;
        this.CastDelay  = 0f;
        this.Lifetime   = 0f;
        this.Spread     = 0f;
    }


    public IEnumerator LifetimeTimer()
    {
        yield return new WaitForSeconds(Lifetime);
        this.gameObject.SetActive(false);
    }
}

// This script is assinged to our 'Generic' Projectile objects. 
// The generic projectile is what a wand will actually launch.