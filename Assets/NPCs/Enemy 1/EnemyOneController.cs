using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOneController : MonoBehaviour
{

    public enum AIMode
    {
        idle = 0,
        trackingPlayer = 1,
        attacking = 2
    }

    public enum Direction
    {
        up = 0,
        right = 1,
        down = 2,
        left = 3
    }

    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject MapGen;   
    [SerializeField] private Rigidbody2D EnemyRB;
    [SerializeField] private Animator EnemyAnimator;

    // Player Projectile Parameters
    private GameObject PlayerProjectile;
    private Rigidbody2D PlayerProjectileRB;

    // Enemy Health Parameters
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private float hitAnimationTime = 0.1f;

    // Enemy Sprite Controllers
    [SerializeField] private SpriteRenderer EnemySprite;
    [SerializeField] private Shader shaderGUItext;
    [SerializeField] private Shader shaderSpritesDefault;

    // Enemy status
    public AIMode status;

    // General Movement Parameters
    public int enemyIdleSpeed = 2;
    public int enemyAttackSpeed = 8;
    public int enemyRunningThreshold = 1;

    // Idle Movement Parameters
    private float chanceToMove;
    private Direction movingDirection;
    [SerializeField] private float movementTimer;
    [SerializeField] private float idleMovementTimer = 2f;
    [SerializeField] private Vector2 directionToPlayer;

    // Enemy Searching Parameters
    [SerializeField] private GameObject Target;
    [SerializeField] private float checkForPlayerTime = 0.2f;
    [SerializeField] private float scanRadius = 5f;
    [SerializeField] private LayerMask scanLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    private Collider2D playerIsWithinRange;
    private RaycastHit2D lineOfSightWithPlayer;

    // Enemy Attcking Parameters
    [SerializeField] private int lungeDamage = 1;
    [SerializeField] private float lungeSpeed = 12f;
    [SerializeField] private float lungeRadius = 7f;
    [SerializeField] private float lungeCoolDownTime = 0.4f;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MapGen = GameObject.FindGameObjectWithTag("Game Object");
        EnemyRB = this.GetComponent<Rigidbody2D>();
        EnemyAnimator = this.GetComponent<Animator>();
        EnemySprite = this.GetComponent<SpriteRenderer>();
        EnemyAnimator.SetTrigger("Idling");

        Physics2D.IgnoreCollision(this.GetComponent<CapsuleCollider2D>(), Player.GetComponent<CapsuleCollider2D>());

        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");

        currentHealth = maxHealth;

        status = AIMode.idle;
        StartCoroutine(PerformScan(checkForPlayerTime));
        StartCoroutine(EnemyIdleMovement(idleMovementTimer));
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player Projectile")
        {

            Destroy(collision.gameObject);

            currentHealth -= 1;

            StartCoroutine(EnemyHitAnimation(hitAnimationTime));
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Player") && (Player.GetComponent<PlayerController>().canTakeDamage == true) && (Player.GetComponent<PlayerController>().rolling == false))
        {
            PlayerController playerControl = Player.GetComponent<PlayerController>();

            playerControl.healthCurrent -= lungeDamage;
            playerControl.healthBar.SetHealth(Player.GetComponent<PlayerController>().healthCurrent);

            playerControl.canTakeDamage = false;
            StartCoroutine(playerControl.PlayerHitAnimation(playerControl.iFrameTime));

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }

        if (status == AIMode.trackingPlayer)
        {
            directionToPlayer = (Vector2)Player.transform.position - (Vector2)this.transform.position;

            EnemyRB.velocity = directionToPlayer.normalized * enemyAttackSpeed;

            if (directionToPlayer.magnitude <= lungeRadius)
            {
                status = AIMode.attacking;

                EnemyRB.velocity = EnemyRB.velocity.normalized * lungeSpeed;

                StartCoroutine(lungeCoolDown(lungeCoolDownTime));
            }
        }

        
        if (EnemyRB.velocity.magnitude > enemyRunningThreshold)
        {
            EnemyAnimator.ResetTrigger("Idling");
            EnemyAnimator.SetTrigger("Running");
        }
        else
        {
            EnemyAnimator.ResetTrigger("Running");
            EnemyAnimator.SetTrigger("Idling");
        }
    }

    IEnumerator lungeCoolDown(float lungeCoolDownTime)
    {
        yield return new WaitForSeconds(lungeCoolDownTime);
        status = AIMode.trackingPlayer;
    }

    IEnumerator PerformScan(float moveUpdateTimeDelay)
    {
        while (true)
        {
            yield return new WaitForSeconds(moveUpdateTimeDelay);

            playerIsWithinRange = Physics2D.OverlapCircle((Vector2)this.transform.position, scanRadius, scanLayerMask);

            if ((playerIsWithinRange) != null && (status != AIMode.attacking))
            {
                lineOfSightWithPlayer = Physics2D.Linecast((Vector2)(this.transform.position), (Vector2)(playerIsWithinRange.transform.position), enemyLayerMask);

                if (lineOfSightWithPlayer.transform.gameObject == Player)
                {
                    Target = playerIsWithinRange.gameObject;
                    status = AIMode.trackingPlayer;
                }
                else if (status == AIMode.trackingPlayer)
                {
                    Target = null;
                    status = AIMode.idle;
                }
            }
        }
    }

    IEnumerator EnemyIdleMovement(float movementTimer)
    {
        while (true)
        {
            yield return new WaitForSeconds(movementTimer);

            if (status == AIMode.idle)
            {
                movingDirection = (Direction)Random.Range(((int)Direction.up), ((int)Direction.left) + 2);

                if (movingDirection == Direction.up)
                {
                    EnemyRB.velocity = new Vector2Int(0, enemyIdleSpeed);
                }
                else if (movingDirection == Direction.right)
                {
                    EnemyRB.velocity = new Vector2Int(enemyIdleSpeed, 0);
                }
                else if (movingDirection == Direction.down)
                {
                    EnemyRB.velocity = new Vector2Int(0, -enemyIdleSpeed);
                }
                else if (movingDirection == Direction.left)
                {
                    EnemyRB.velocity = new Vector2Int(-enemyIdleSpeed, 0);
                }
                else
                {
                    EnemyRB.velocity = new Vector2Int(0, 0);
                }
            }
        }
    }

    /*
    IEnumerator AttackMovement(float attackMovementTimer)
    {
        while (true)
        {
            yield return new WaitForSeconds(attackMovementTimer);

            directionToPlayer = (Vector2)Player.transform.position - (Vector2)this.transform.position;
            EnemyRB.velocity = directionToPlayer.normalized * enemyAttackSpeed;
        }
    }
    */


    IEnumerator EnemyHitAnimation(float hitAnimationTime)
    {
        EnemySprite.material.shader = shaderGUItext;
        EnemySprite.color = Color.white;

        yield return new WaitForSeconds(hitAnimationTime);

        EnemySprite.material.shader = shaderSpritesDefault;
        EnemySprite.color = Color.white;
    }


    
}
