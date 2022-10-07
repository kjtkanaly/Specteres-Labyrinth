using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : Wand
{
    public WandOrientation WandDirection;
    public List<Spell> SpellList;
    public Spell temp;

    private bool CanCastSpell = true;
    private int  SpellIndex = 0;

    private void Awake()
    {
        SpellList = new List<Spell>();
        SpellIndex = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && CanCastSpell)
        {
            if (SpellList[SpellIndex].Type == SpellType.Projectile)
            {
                GameObject Spell = ObjectPooling.SharedInstance.GetPooledObject();

                if (Spell != null)
                {
                    Spell.SetActive(true);
                    Spell.GetComponent<BouncingBallPhysics>().Parent         = this.transform;
                    Spell.GetComponent<BouncingBallPhysics>().MouseDirection = WandDirection.MousePos;
                    Spell.GetComponent<BouncingBallPhysics>().OnActive();
                }

                StartCoroutine(GeneralTimer(FireRate));
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
