using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectileSpell : Spell
{
    // RB: Object's RigidBody
    // Trans: Object's Transform
    public Rigidbody2D      RB;
    public Transform        Trans;
    public SpriteRenderer   Spr;
    public CircleCollider2D Col;
    public BoxCollider2D[]  PlayerColliders;

    public void Awake()
    {
        RB      = this.GetComponent<Rigidbody2D>();
        Trans   = this.GetComponent<Transform>();        
        Spr     = this.GetComponentInChildren(typeof(SpriteRenderer), true) as SpriteRenderer;
        Col     = this.GetComponent<CircleCollider2D>();
        PlayerColliders = GameObject.FindGameObjectWithTag("Player").GetComponents<BoxCollider2D>();
    }

    public void OnEnable()
    {
        Debug.Log(PlayerColliders.Length);
        foreach (BoxCollider2D playerCol in PlayerColliders)
        {
            Physics2D.IgnoreCollision(Col, playerCol);
        }
    }

    public void OnCollisionEnter2D(Collision2D col)
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