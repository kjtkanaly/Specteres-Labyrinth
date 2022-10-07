using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpell : Spell
{
    // Speed: Pixels per frame
    // Damage: Cost to a creature's health
    // Cast Delay: Time delay after casting
    // Lifetime: Time the projectile will exisit
    // Spread: The angle range the projectle might fire at
    public float Speed      = 100f;
    public float Damge      = 10f;
    public float CastDelay  = 1f;
    public float Lifetime   = 20f;
    public float Spread     = 1f;

}
