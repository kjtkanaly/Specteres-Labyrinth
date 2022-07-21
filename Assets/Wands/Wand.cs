using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour
{

	public float castDelay;   	// Seconds between spell casts
	public float rechargeTime;	// Seconds between restarting spell chain
	public float castSpread;		// Limit on projectile spread, degrees
	
	public int spellSlots; 		// Number of spells we can fit in the wand
	
	public List<Spell> Spells = new List<Spell>(); // Lists of spells in the Wand
	
	
}