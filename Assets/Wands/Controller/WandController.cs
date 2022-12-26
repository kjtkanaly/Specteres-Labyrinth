using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : Wand
{
    public WandOrientation WandDirection;
    public ObjectPool ObjectPool;
    public MainGameControl MainGameCtrl;
    public Transform PlayerTrans;
    public PlayerControllerTwo PlayerCtrl;
    public List<Spell> SpellList;

    public float projecitleAngle;

    private bool CanCastSpell = true;
    private int  SpellIndex = 0;

    private void Awake()
    {
        //SpellList = new List<Spell>();
        SpellIndex = 0;

        PlayerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerCtrl = GameObject.FindGameObjectWithTag("Player").
                     GetComponent<PlayerControllerTwo>();

        GameObject MainGameObj = GameObject.FindGameObjectWithTag("Main Game");
        ObjectPool = MainGameObj.GetComponent<ObjectPool>();
        MainGameCtrl = MainGameObj.GetComponent<MainGameControl>();
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    private void Update()
    {
        if ((Input.GetMouseButton(0)) && 
            (CanCastSpell) && 
            (SpellList.Count != 0) &&
            (PlayerCtrl.currentMagicMana > 0))
        {
            // Projectile Spells
            if (SpellList[SpellIndex].Type == Spell.SpellType.Projectile)
            {
                Spell CurrentProjectile = SpellList[SpellIndex];
                GenericProjectileSpell GenericSpell = 
                ObjectPool.GetObjectFromThePool<GenericProjectileSpell>(
                    MainGameCtrl.GenericProjectilePool);

                if (GenericSpell != null)
                {
                    // Activate the spell
                    GenericSpell.gameObject.SetActive(true);

                    // Setting the spell's spawn location
                    GenericSpell.transform.SetParent(
                        this.transform.GetChild(0).transform);
                    GenericSpell.transform.localPosition = new Vector3(
                                                           0f,
                                                           0.63f);
                    GenericSpell.transform.SetParent(null);

                    // Added wand spread
                    projecitleAngle = Vector2.SignedAngle(
                                      new Vector2(1f, 0),
                                      WandDirection.MousePos.normalized);
                    projecitleAngle += Random.Range(-Spread, Spread);

                    // Setting the spell's velocity
                    GenericSpell.RB.velocity = 
                        DegreeToVector2(projecitleAngle) * 
                        CurrentProjectile.Speed;

                    // Setting the spell's parameters
                    GenericSpell.Name = CurrentProjectile.Name;
                    GenericSpell.ManaDrain = CurrentProjectile.ManaDrain;
                    GenericSpell.Speed = CurrentProjectile.Speed;
                    GenericSpell.Damage = CurrentProjectile.Damage;
                    GenericSpell.CastDelay = CurrentProjectile.CastDelay;
                    GenericSpell.Lifetime = CurrentProjectile.Lifetime;
                    GenericSpell.Spread = CurrentProjectile.Spread;
                    GenericSpell.particleTimeDelay = 
                        CurrentProjectile.particleTimeDelay;
                    GenericSpell.particleSpeedPerecent = 
                        CurrentProjectile.particleSpeedPerecent;
                    GenericSpell.particleColor = 
                        CurrentProjectile.particleColor;
                    GenericSpell.CanBounce = 
                        CurrentProjectile.CanBounce;

                    // Setting the spell's physic
                    GenericSpell.RB.sharedMaterial.bounciness = 
                        CurrentProjectile.Bounce;
                    GenericSpell.RB.sharedMaterial.friction = 
                        CurrentProjectile.Friction;

                    // Setting the projectile's sprite
                    GenericSpell.Spr.sprite = CurrentProjectile.SpriteImage;
                    
                    // Starting the lifetime timer
                    StartCoroutine(GenericSpell.LifetimeTimer());
                }
                // Update Player current mana currently
                PlayerCtrl.currentMagicMana -= SpellList[SpellIndex].ManaDrain;
                PlayerCtrl.ManaBarCtrl.SetMana(PlayerCtrl.currentMagicMana);

                CanCastSpell = false;
                StartCoroutine(GeneralTimer(CurrentProjectile.CastDelay));
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
// 3. When the wand casts a projectile spell we will pull a generic projectile
//    from the generic projectile pool
// 4. We will then set the generic projectile's properties to the wands 
//    current projectile spell
// 5. The projectile will then be enabled, and we iterate to the next spell 
//    in the wand's active spell list
