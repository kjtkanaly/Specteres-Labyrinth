using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerTwo : MonoBehaviour
{
    public Rigidbody2D RB;
    public Collider2D FeetCollider;
    public Collider2D HeadCollider;
    public Collider2D RightCollider;
    public Collider2D LeftCollider;
    public GenericParticle FlyingParticle;
    public ObjectPool ObjectPooling;
    public LevitateBarScript LevitationBarCtrl;
    public ManaBarScript ManaBarCtrl;

    private List<GenericParticle> FlyingParticlePool;
    public Vector2 PlayerVelocity = new Vector2(0f,0f);

    public float mass = 10f;
    public float FrictionRate = 1f;
    public float walkingSpeed = 10f;
    public float gravityConstant = -10f;
    public float LevitationForce = 150f;
    public float terminalVelocity = -15f;
    public float maxHorizontalSpeed = 10f;
    public float flyingParticleChance = 0.1f; 
    public float magicManaRechargeDelay = 0.1f;

    public int maxMagicMana = 100;
    public int magicManaStepSize = 2;
    public int maxLevitationMana = 100;
    public int leviationStepSize = 1;
    public int FlyingParticleCount = 100;
    public int currentLevitationMana;
    public int currentMagicMana;

    public bool AddGravity = true;
    public bool isHittingHead = false;
    public bool isRunningLeftIntoWall = false;
    public bool isRunningRightIntoWall = false;
    public bool canRechargeMagicMana = true;


    private void Start()
    {
        FlyingParticlePool = new List<GenericParticle>();
        FlyingParticlePool = ObjectPooling.setupInstancePool<GenericParticle>(
                             FlyingParticle, 
                             FlyingParticleCount);

        currentLevitationMana = maxLevitationMana;
        LevitationBarCtrl.SetMaxLevitation(maxLevitationMana);

        currentMagicMana = maxMagicMana;
        ManaBarCtrl.SetMaxMana(maxMagicMana);

        StartCoroutine(RechargeMagicManaTimer());
    }


    public IEnumerator RechargeMagicManaTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(magicManaRechargeDelay);
            if (!(Input.GetMouseButton(0)) && 
                (currentMagicMana < maxMagicMana))
            {
                currentMagicMana += magicManaStepSize;
            }

            if (currentMagicMana > maxMagicMana)
            {
                currentMagicMana = maxMagicMana;
            }

            ManaBarCtrl.SetMana(currentMagicMana);
        }
    }


    // Update is called once per frame
    void Update()
    {

        // Check if player jumped
        if ((Input.GetKey(KeyCode.Space)) && (currentLevitationMana > 0))
        {
            // Update Y-axis velocity
            if (isHittingHead == false)
            {
                PlayerVelocity += new Vector2(
                                  0f, 
                                  (LevitationForce / mass) * Time.deltaTime);
            }
            else
            {
                PlayerVelocity = new Vector2(PlayerVelocity.x, 0f);
            }   

            // Update leviation bar
            /* if ((currentLevitationMana <= maxLevitationMana) && 
                (LevitationBarCtrl.FillImage.color.a != 1f))
            {
                LevitationBarCtrl.StartCoroutine(LevitationBarCtrl.fadeInCoroutine);
            } */

            currentLevitationMana -= leviationStepSize;
            LevitationBarCtrl.SetLeviataion(currentLevitationMana);

            // Explel Leviation Particle
            float particleChance = Random.Range(0f, 1f);
            if (particleChance > flyingParticleChance)
            {
                GenericParticle FlyingParticleObj = 
                ObjectPooling.GetObjectFromThePool<GenericParticle>(
                FlyingParticlePool);

                if (FlyingParticleObj != null)
                {
                    FlyingParticleObj.gameObject.SetActive(true);
                }
            }
        }
        else 
        {
            if (AddGravity)
            {
                PlayerVelocity.y = Mathf.MoveTowards(
                                   PlayerVelocity.y, 
                                   terminalVelocity, 
                                   gravityConstant * Time.deltaTime);
            }
            else
            {
                PlayerVelocity.y = 0;
            }

            // Recharge Levitation Mana
            if (currentLevitationMana < maxLevitationMana)
            {
                currentLevitationMana += leviationStepSize;
                LevitationBarCtrl.SetLeviataion(currentLevitationMana);

                /* if (currentLevitationMana == maxLevitationMana)
                {
                    LevitationBarCtrl.StartCoroutine(LevitationBarCtrl.fadeOutCoroutine);
                } */
            }
        }

        // Check for left/right movement
        // Right
        if ((Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.A))) 
        {
            if (isRunningRightIntoWall == false)
            {
                PlayerVelocity = new Vector2(walkingSpeed, PlayerVelocity.y);
            }
            else
            {
                PlayerVelocity = new Vector2(0, PlayerVelocity.y);
            }
        }
        // Left
        else if (!(Input.GetKey(KeyCode.D)) && (Input.GetKey(KeyCode.A))) 
        {
            if (isRunningLeftIntoWall == false)
            {
                PlayerVelocity = new Vector2(-walkingSpeed, PlayerVelocity.y);
            }
            else
            {
                PlayerVelocity = new Vector2(0, PlayerVelocity.y);
            }
        }
        // Left and Right
        else if ((Input.GetKey(KeyCode.D)) && (Input.GetKey(KeyCode.A)))
        {
            PlayerVelocity = new Vector2(0, PlayerVelocity.y);
        }
        // No input
        else
        {
            PlayerVelocity.x = Mathf.MoveTowards(
                               PlayerVelocity.x, 
                               0, 
                               FrictionRate * Time.deltaTime);
        }
        
        // Clamp Player Velocity
        PlayerVelocity.y = Mathf.Clamp(PlayerVelocity.y, 
                                       -terminalVelocity, 
                                       terminalVelocity);
        PlayerVelocity.x = Mathf.Clamp(PlayerVelocity.x, 
                                       -maxHorizontalSpeed, 
                                       maxHorizontalSpeed);

        // Update RB
        RB.velocity = PlayerVelocity;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if ((FeetCollider == col.otherCollider) || 
            (FeetCollider == col.collider))
        {
            Debug.Log("Touchdown!");
            AddGravity = false;
            PlayerVelocity = new Vector2(PlayerVelocity.x, 0f);
        }

        if ((HeadCollider == col.otherCollider) || 
            (HeadCollider == col.collider))
        {
            Debug.Log("Ouch Me Head");
            isHittingHead = true;
            PlayerVelocity = new Vector2(PlayerVelocity.x, 0f);
        }

        if ((RightCollider == col.otherCollider) || 
            (LeftCollider == col.collider))
        {
            Debug.Log("Right Oof!");
            isRunningRightIntoWall = true;
            PlayerVelocity = new Vector2(0f, PlayerVelocity.y);
        }

        if ((LeftCollider == col.otherCollider) || 
            (LeftCollider == col.collider))
        {
            Debug.Log("Left Oof!");
            isRunningLeftIntoWall = true;
            PlayerVelocity = new Vector2(0f, PlayerVelocity.y);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if ((FeetCollider == col.otherCollider) || 
            (FeetCollider == col.collider))
        {
            Debug.Log("Liftoff");
            AddGravity = true;
        }

        if ((HeadCollider == col.otherCollider) || 
            (HeadCollider == col.collider))
        {
            Debug.Log("Phew");
            isHittingHead = false;
        }

        if ((RightCollider == col.otherCollider) || 
            (LeftCollider == col.collider))
        {
            Debug.Log("Right Wee!");
            isRunningRightIntoWall = false;
        }

        if ((LeftCollider == col.otherCollider) || 
            (LeftCollider == col.collider))
        {
            Debug.Log("Left Wee!");
            isRunningLeftIntoWall = false;
        }
    }

}
