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
    public bool Shuffle = false;
    public bool Randomize = false;

    public int CastRate = 0;
    public int MaxMana = 0;
    public int ManaRechargeStepSize = 0;
    public int SpellCapacity = 0;

    public float CastDelay = 0;
    public float ProjectileSpread = 0; 

    public Vector2Int MaxManaRange = new Vector2Int(0, 1);
    public Vector2Int ManaRechargeStepSizeRange = new Vector2Int(0, 1);
    public Vector2Int SpellCapacityRange = new Vector2Int(0, 1);
    public Vector2 CastDelayRange = new Vector2(0, 1f);
    public Vector2 ProjectileSpreadRange = new Vector2(0, 1f);
    
    private void Awake()
    {
        if (Randomize)
        {
            MaxMana = Random.Range(MaxManaRange.x, MaxManaRange.y);
            ManaRechargeStepSize = Random.Range(
                                   ManaRechargeStepSizeRange.x, 
                                   ManaRechargeStepSizeRange.y);
            SpellCapacity = Random.Range(
                            SpellCapacityRange.x, 
                            SpellCapacityRange.y);
            CastDelay = Random.Range(CastDelayRange.x, CastDelayRange.y);
            ProjectileSpread = Random.Range(
                               ProjectileSpreadRange.x,
                               ProjectileSpreadRange.y);
        }
    }
}
