using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : Wand
{
    public WandOrientation WandDirection;
    public ObjectPool SpellPool;
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
                GameObject GenericSpellObj = SpellPool.GetObjectFromThePool();

                if (GenericSpellObj != null)
                {
                    // Activate the spell
                    GenericSpellObj.SetActive(true);
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
