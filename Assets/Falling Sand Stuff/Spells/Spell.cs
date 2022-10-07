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

    // Type: The type of spell
    // Name: Name of the spell
    // ManaDrain: Amount of mana that the spell will drain on each cast
    public SpellType Type;
    public string    Name;
    public float     ManaDrain;

    
}
