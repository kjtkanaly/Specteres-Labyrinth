using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectileSpell : ProjectileSpell
{
    // RB: Object's RigidBody
    // Trans: Object's Transform
    public Rigidbody2D RB;
    public Transform   Trans;

    public void Awake()
    {
        BallRB    = this.GetComponent<Rigidbody2D>();
        BallTrans = this.GetComponent<Transform>();        
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
}

// This script is assinged to our 'Generic' Projectile objects. 
// The generic projectile is what a wand will actually launch.