using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectileSpell : Spell
{
    // RB: Object's RigidBody
    // Trans: Object's Transform
    public Rigidbody2D    RB;
    public Transform      Trans;
    public SpriteRenderer Spr;

    public void Awake()
    {
        RB      = this.GetComponent<Rigidbody2D>();
        Trans   = this.GetComponent<Transform>();        
        Spr     = this.GetComponentInChildren(typeof(SpriteRenderer), true) as SpriteRenderer;
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