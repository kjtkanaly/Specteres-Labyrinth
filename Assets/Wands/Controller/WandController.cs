using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour
{
    public Wand WandProperties;
    public SpriteRenderer WandSprite;
    public Animator AnimationCtrl;
    public WandOrientation WandDirection;

    public MainGameControl MainGameCtrl;
    public ObjectPool ObjectPool;
    public ManaBarScript ManaBarCtrl;

    public Transform PlayerTrans;
    public PlayerControllerTwo PlayerCtrl;
    public InvetorySytem InvetoryCtrl;

    public List<Spell> SpellList;

    public float projecitleAngle;

    private int  SpellIndex = 0;
    public int currentMagicMana;

    public bool CanBePickedUp = false;
    private bool CanCastSpell = true;

    // ------------------------------------------------------------------------
    private void Awake()
    {
        WandSprite = GetComponentInChildren(typeof(SpriteRenderer)) 
                     as SpriteRenderer;

        currentMagicMana = WandProperties.MaxMana;

        ManaBarCtrl.SetMaxMana(WandProperties.MaxMana);

        PlayerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerCtrl = GameObject.FindGameObjectWithTag("Player").
                     GetComponent<PlayerControllerTwo>();
        InvetoryCtrl = GameObject.FindGameObjectWithTag("Main Game").
                       GetComponent<InvetorySytem>();

        GameObject MainGameObj = GameObject.FindGameObjectWithTag("Main Game");
        ObjectPool = MainGameObj.GetComponent<ObjectPool>();
        MainGameCtrl = MainGameObj.GetComponent<MainGameControl>();
    }
    
    // ------------------------------------------------------------------------
    private void OnEnable()
    {
        SpellIndex = 0;

        StartCoroutine(RechargeManaTimer());

    }

    // ------------------------------------------------------------------------
    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    // ------------------------------------------------------------------------
    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    // ------------------------------------------------------------------------
    public IEnumerator RechargeManaTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!(Input.GetMouseButton(0)) && 
                (currentMagicMana < WandProperties.MaxMana))
            {
                currentMagicMana += WandProperties.ManaRechargeSpeed;
            }

            if (currentMagicMana > WandProperties.MaxMana)
            {
                currentMagicMana = WandProperties.MaxMana;
            }

            if (WandProperties.wandState == Wand.WandState.Equipped)
            {
                ManaBarCtrl.SetMana(currentMagicMana);
            }
        }
    }

    // ------------------------------------------------------------------------
    public void CastProjectileSpell()
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
                projecitleAngle += Random.Range(
                                -WandProperties.ProjectileSpread, 
                                WandProperties.ProjectileSpread);

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
                GenericSpell.StartCoroutine(GenericSpell.LifetimeTimer());
            }
            // Update Player current mana currently
            currentMagicMana -= SpellList[SpellIndex].ManaDrain;
            ManaBarCtrl.SetMana(currentMagicMana);

            CanCastSpell = false;
            StartCoroutine(GeneralTimer(CurrentProjectile.CastDelay));
        }
    }

    // ------------------------------------------------------------------------
    private void WandIdleOnGround()
    {
        AnimationCtrl.SetBool("Idle", true);
    }

    // ------------------------------------------------------------------------
    private void Update()
    {
        if (WandProperties.wandState == Wand.WandState.Equipped)
        {
            if ((Input.GetMouseButton(0)) && (CanCastSpell) && 
                (SpellList.Count != 0) && (currentMagicMana > 0))
            {
                CastProjectileSpell();
            }
        }
        else if (WandProperties.wandState == Wand.WandState.OnTheGround)
        {
            WandIdleOnGround();

            // Check if the player it trying to pick up the wand
            if ((Input.GetKeyDown(KeyCode.E)) && (CanBePickedUp))
            {
                PickUpWand();
            }
        }
    }

    // ------------------------------------------------------------------------
    public void PickUpWand()
    {
        Debug.Log("Picked Up!");
        WandProperties.wandState = Wand.WandState.InInventory;
        AnimationCtrl.SetBool("Idle", false);
    }

    // ------------------------------------------------------------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((WandProperties.wandState == Wand.WandState.OnTheGround) &&
            (other.tag == "Item Range"))
        {
            Debug.Log("Can be picked up!");
            CanBePickedUp = true;
        }   
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((WandProperties.wandState == Wand.WandState.OnTheGround) &&
            (other.tag == "Item Range"))
        {
            Debug.Log("Can't be picked up...");
            CanBePickedUp = false;
        }   
    }

    // ------------------------------------------------------------------------
    public IEnumerator GeneralTimer(float Duration)
    {
        yield return new WaitForSeconds(Duration);

        CanCastSpell = true;
    }
}