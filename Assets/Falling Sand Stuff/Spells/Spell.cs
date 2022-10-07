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

    // SpellPrefab: The gameobject that will be used for projectles
    // Type: The type of spell
    // ManaDrain: Amount of mana that the spell will drain on each cast
    public GameObject SpellPrefab;
    public SpellType Type;
    public float ManaDrain;

    
}
