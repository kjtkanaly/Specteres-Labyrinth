using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public enum SpellType
    {
        Projectile,
        DamageMod,
        CastMod,
        PathMod
    }

    ////////////////////////////////
    // General Spell Parameters:
    ////////////////////////////////
    // Type: The type of spell
    // Name: Name of the spell
    // ManaDrain: Amount of mana that the spell will drain on each cast
    public SpellType Type;
    public string    Name;
    public float     ManaDrain;

    ////////////////////////////////
    // Projectile Spell Parameters:
    ////////////////////////////////
    // Speed: Pixels per frame
    // Damage: Cost to a creature's health
    // Cast Delay: Time delay after casting
    // Lifetime: Time the projectile will exisit
    // Spread: The angle range the projectle might fire at
    // Bounce: The amount of bounce (0 - 1)
    // Friction: The amount of surface friction (0 - 1)
    public Sprite SpriteImage;

    public float Speed = 0f;
    public float Damage = 0f;
    public float CastDelay = 0f;
    public float Lifetime = 0f;
    public float Spread = 0f;
    public float Bounce = 0f;
    public float Friction = 0f;
    public float particleTimeDelay = 0f;
    public float particleSpeedPerecent = 0f;
    public Color particleColor = new Color(1f, 1f, 1f, 1f);
    public bool CanBounce  = false;

    //////////////////////////////////
    // Cast Modifer Spell Parameters:
    //////////////////////////////////
    // MultiCast: How many additional cast will be made
    // SpreadMod: Change in the next spell's Spread
    public int   MultiCast    = 2;
    public float SpreadMod    = 0f;
    
}
