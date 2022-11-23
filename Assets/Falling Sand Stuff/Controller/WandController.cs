using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : Wand
{
    public WandOrientation WandDirection;
    public List<Spell> SpellList;

    private bool CanCastSpell = true;
    private int  SpellIndex = 0;

    private void Awake()
    {
        //SpellList = new List<Spell>();
        SpellIndex = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && CanCastSpell && SpellList.Count != 0)
        {
            if (SpellList[SpellIndex].Type == Spell.SpellType.Projectile)
            {
                Spell CurrentProjectile = SpellList[SpellIndex];
                GenericProjectileSpell GenericSpell = SpellPooling.SharedInstance.GetPooledSpell();

                if (GenericSpell != null)
                {
                    // Activate the spell
                    GenericSpell.gameObject.SetActive(true);

                    // Setting the spell's spawn location
                    GenericSpell.Trans.SetParent(this.transform.GetChild(0).transform);
                    GenericSpell.Trans.localPosition = new Vector3(0f, 0.63f);
                    GenericSpell.Trans.SetParent(null);

                    // Setting the spell's velocity
                    GenericSpell.RB.velocity = WandDirection.MousePos.normalized * CurrentProjectile.Speed;

                    // Setting the spell's parameters
                    GenericSpell.Name       = CurrentProjectile.Name;
                    GenericSpell.ManaDrain  = CurrentProjectile.ManaDrain;
                    GenericSpell.Speed      = CurrentProjectile.Speed;
                    GenericSpell.Damage     = CurrentProjectile.Damage;
                    GenericSpell.CastDelay  = CurrentProjectile.CastDelay;
                    GenericSpell.Lifetime   = CurrentProjectile.Lifetime;
                    GenericSpell.Spread     = CurrentProjectile.Spread;
                    GenericSpell.CanBounce  = CurrentProjectile.CanBounce;

                    // Setting the spell's physic
                    GenericSpell.RB.sharedMaterial.bounciness = CurrentProjectile.Bounce;
                    GenericSpell.RB.sharedMaterial.friction = CurrentProjectile.Friction;

                    // Setting the projectile's sprite
                    GenericSpell.Spr.sprite = CurrentProjectile.SpriteImage;

                    // Starting the lifetime timer
                    StartCoroutine(GenericSpell.LifetimeTimer());
                }

                StartCoroutine(GeneralTimer(CastRate));
                CanCastSpell = false;
            }
            
        }
    }

    public IEnumerator GeneralTimer(float Duration)
    {
        yield return new WaitForSeconds(Duration);

        CanCastSpell = true;
    }
}


// Casting Spells works
// 1. The palyer object will have a Generic Projectile Spell Pool
// 2. Each wand has a spell list
// 3. When the wand casts a projectile spell we will pull a generic projectile from the pool
// 4. We will then set the generic projectile's properties to the current projectile spell
// 5. The projectile will then be launched, and we iterate to the next spell in the wand list