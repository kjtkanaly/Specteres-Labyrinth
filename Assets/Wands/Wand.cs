using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour
{
    // Shuffle: Will the wand randomly select spells to cast
    // CastRate: How many spells are cast at a time
    // ManaMax: The wands mana capacity
    // ManaCharge: The amount of mana the wand will revover per second
    // Capacity: The number of spells the wand can hold
    // CastDelay: The amount of time the wand will wait between spells
    // RechargeTime: The amount of time the wand will wait after the final spell
    // Spread: The potential range your projectiles
    public bool  Shuffle        = false;
    public int   CastRate       = 0;
    public int   ManaMax        = 0;
    public int   ManaCharge     = 0;
    public int   Capacity       = 0;
    public float CastDelay      = 0;
    public float RechargeTime   = 0;
    public float Spread         = 0;  
    
}
