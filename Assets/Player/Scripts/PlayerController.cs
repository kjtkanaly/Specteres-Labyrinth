using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Movement
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Collider2D playerBoundaryCollider;
    [SerializeField] private Vector2 velocity = new Vector2(0, 0);
    [SerializeField] private Vector3 rollingAngluarVelocity = new Vector3(0, 0, 1000f);
    [SerializeField] private float acceleration = 300;
    [SerializeField] private float veloictyCap = 10;
    [SerializeField] private float rollTime = 0.25f;
    [SerializeField] private float rollingVelocity = 15;
    [SerializeField] private float runningSpeed;
    public bool rolling = false;
    public bool canMove = true;

    // Player Animation
    [SerializeField] private Animator playerAnime;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Shader shaderGUItext;
    [SerializeField] private Shader shaderSpritesDefault;

    // Reticle Parameters
    [SerializeField] private Transform avgBetweenPlyAndReticle;
    [SerializeField] private float avgDiffMax = 3f;
    [SerializeField] private Transform reticlePos;

    // Weapon Parameters
	public WandLogic wandController;
    [SerializeField] private Transform weaponHilt;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private GameObject shotPrefab;
    private Vector2 reticlePosRelativeToHilt;

    // UI Parameters
    public ManaBarScript manaBar;
    public HealthBarScript healthBar;
    public int manaMax = 100;
    public int manaCurrent = 100;
    public int healthMax = 10;
    public int healthCurrent = 10;

    // Player Defence/I Frame Parameters
    public bool canTakeDamage = true;
    public float iFrameTime = 0.1f;

    // Attacking Parameters
    private float timeSinceLastManaUpdate = 0f;
    private bool canShoot = true;
    public float shotTimeDelta = 0.2f;
    public float manaTimeDelta = 0.2f;


    private void Start()
    {
        Cursor.visible = false;
        canTakeDamage = true;

        manaBar.SetMaxMana(manaMax);
        healthBar.SetMaxHealth(healthMax);

        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        if (canMove == true)
        {


            reticlePos.position = getMouseWorldPosition();

            avgBetweenPlyAndReticle.localPosition = Vector3.ClampMagnitude((reticlePos.localPosition), avgDiffMax);

            reticlePosRelativeToHilt = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - weaponHilt.transform.position;
            weaponHilt.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(reticlePosRelativeToHilt.y, reticlePosRelativeToHilt.x));

            if (Mathf.Sign(Mathf.Rad2Deg * Mathf.Atan2(reticlePosRelativeToHilt.y, reticlePosRelativeToHilt.x)) > 0)
            {
                weaponSprite.sortingOrder = -1;
            }
            else
            {
                weaponSprite.sortingOrder = 1;
            }

            if (Mathf.Abs(Mathf.Rad2Deg * Mathf.Atan2(reticlePosRelativeToHilt.y, reticlePosRelativeToHilt.x)) < 90f)
            {
                playerSprite.flipX = false;
                weaponHilt.localPosition = new Vector2(-0.19f, weaponHilt.localPosition.y);
                weaponSprite.flipX = false;
            }
            else
            {
                playerSprite.flipX = true;
                weaponHilt.localPosition = new Vector2(0.19f, weaponHilt.localPosition.y);
                weaponSprite.flipX = true;
            }


            //////////////////////////////////////////////////////
            // Charecter Movement
            if (rolling == false)
            {
                if (Input.GetKey("d"))
                {
                    velocity.x += Time.deltaTime * acceleration;
                }
                if (Input.GetKey("a"))
                {
                    velocity.x += Time.deltaTime * -acceleration;
                }
                if (Input.GetKey("w"))
                {
                    velocity.y += Time.deltaTime * acceleration;
                }
                if (Input.GetKey("s"))
                {
                    velocity.y += Time.deltaTime * -acceleration;
                }

                if (!Input.GetKey("d") && velocity.x > 0f)
                {
                    velocity.x -= Time.deltaTime * acceleration;
                    velocity.x = Mathf.Clamp(velocity.x, 0f, veloictyCap);
                }
                if (!Input.GetKey("a") && velocity.x < 0f)
                {
                    velocity.x += Time.deltaTime * acceleration;
                    velocity.x = Mathf.Clamp(velocity.x, -veloictyCap, 0f);
                }
                if (!Input.GetKey("w") && velocity.y > 0f)
                {
                    velocity.y -= Time.deltaTime * acceleration;
                    velocity.y = Mathf.Clamp(velocity.y, 0f, veloictyCap);
                }
                if (!Input.GetKey("s") && velocity.y < 0f)
                {
                    velocity.y += Time.deltaTime * acceleration;
                    velocity.y = Mathf.Clamp(velocity.y, -veloictyCap, 0f);
                }

                velocity.x = Mathf.Clamp(velocity.x, -veloictyCap, veloictyCap);
                velocity.y = Mathf.Clamp(velocity.y, -veloictyCap, veloictyCap);

                playerRB.velocity = Vector2.ClampMagnitude(new Vector2(velocity.x, velocity.y), veloictyCap);
            }


            //////////////////////////////////////////////////////
            // Recharging Mana
            if ((manaCurrent < manaMax) && !(Input.GetMouseButton(0)))
            {
                timeSinceLastManaUpdate += Time.deltaTime;

                if (timeSinceLastManaUpdate >= manaTimeDelta)
                {
                    //Debug.Log("Recharging");
                    manaCurrent += 1;
                    manaBar.SetMana(manaCurrent);
                    timeSinceLastManaUpdate = 0f;

                }
            }


            //////////////////////////////////////////////////////
            // Shooting Logic
            if ((Input.GetMouseButton(0)) && (rolling == false) && (canShoot == true) && (manaCurrent > 0))
            {
				WandController.playerIsCasting = true;
				
				/*
                GameObject bullet = (GameObject)Instantiate(shotPrefab);
                Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), playerBoundaryCollider);

                manaCurrent -= 1;
                manaBar.SetMana(manaCurrent);

                canShoot = false;
                StartCoroutine(shootingTimer(shotTimeDelta));
				*/
            }
			if ((WandController.playerIsCasting == true) && (!Input.GetMouseButton(0)))
			{
				wandController.playerIsCasting = false;
			}
			

            //////////////////////////////////////////////////////
            // Player Rolling
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //playerAnime.ResetTrigger("Idle");
                //playerAnime.ResetTrigger("Running");

                rolling = true;
                playerAnime.SetBool("Roll", true);
                weaponSprite.color = new Color(weaponSprite.color.r, weaponSprite.color.g, weaponSprite.color.b, 0);
                StartCoroutine(rollingTimer(rollTime));

                if (!(playerRB.velocity.magnitude > runningSpeed))
                {
                    playerRB.velocity = (rollingVelocity * reticlePosRelativeToHilt.normalized);
                }
                else
                {
                    playerRB.velocity = (rollingVelocity * new Vector2(playerRB.velocity.x, playerRB.velocity.y).normalized);
                }
            }

            //////////////////////////////////////////////////////
            // Player Running Animation
            playerAnime.SetFloat("Movement Magnitude", playerRB.velocity.magnitude);
        }
    }

    public IEnumerator PlayerHitAnimation(float hitAnimationTime)
    {
        playerSprite.material.shader = shaderGUItext;
        playerSprite.color = Color.white;

        yield return new WaitForSeconds(hitAnimationTime);

        canTakeDamage = true;

        playerSprite.material.shader = shaderSpritesDefault;
        playerSprite.color = Color.white;
    }

    IEnumerator rollingTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        rolling = false;
        playerAnime.SetBool("Roll", false);
        weaponSprite.color = new Color(weaponSprite.color.r, weaponSprite.color.g, weaponSprite.color.b, 1);
        //playerAnime.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        //playerAnime.SetBool("Roll", false);
    }

    IEnumerator shootingTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canShoot = true;
    }

    public static Vector3 getMouseWorldPosition()
    {
        Camera worldCamera = Camera.main;
        Vector3 vec = worldCamera.ScreenToWorldPoint(Input.mousePosition);

        vec.z = 10f;
        return vec;
    }
}


/*
 * if (!(Input.GetKey("d")) && !(Input.GetKey("a")))
        {
            if (velocity.x > 0f)
            {
                Debug.Log("Stopping");
                velocity.x -= Time.deltaTime * acceleration;
                velocity.x = Mathf.Clamp(velocity.x, 0f, veloictyCap);
            }
            else if (velocity.x < 0f)
            {
                Debug.Log("Stopping");
                velocity.x += Time.deltaTime * acceleration;
                velocity.x = Mathf.Clamp(velocity.x, -veloictyCap, 0f);
            }
        }
        if (!(Input.GetKey("w")) && !(Input.GetKey("s")))
        {
            if (velocity.y > 0f)
            {
                Debug.Log("Stopping");
                velocity.y -= Time.deltaTime * acceleration;
                velocity.y = Mathf.Clamp(velocity.y, 0f, veloictyCap);
            }
            else if (velocity.y < 0f)
            {
                Debug.Log("Stopping");
                velocity.y += Time.deltaTime * acceleration;
                velocity.y = Mathf.Clamp(velocity.y, -veloictyCap, 0f);
            }
        }
 */