using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerBranch : MonoBehaviour
{

	public enum States
    {
		Idle,
		Roam,
		ChasePlayer,
		Attack
	}

	public enum Direction
    {
		Up,
		Right,
		Down,
		Left
    }

	private float RoamingSpeed = 4f;
	private float ChasingSpeed = 6f;
	private float RoamDistanceMinimum = 1f;
	private float RoamDistanceMaximum = 4f;
	private float ChasePlayerRange = 15f;
	private int   AttackDamage = 2;
	private int   FramesToDamagePlayer = 1;
	private bool  ReadyForNewRoamDirection = true;
	public  bool  CanDamagePlayer = false;

	private GameObject        player;
	private PlayerController  playerController;
	private CapsuleCollider2D playerCollider;

	private RaycastHit2D     hit;
	private LayerMask        mask;
	private IEnumerator      RoamingTimerInstance;
	private IEnumerator      DamagePlayerFrameDelayInstance;
	public  States           State;
	public  Vector3          StartingPos;
	public  Vector3          RoamPosition;
	private float            RoamMaxTime;
	private float            MaxRoamingStep;
	private float            MaxChaseStep;
	private float            RoamDistance;
	public  float            DistanceToPlayer;
	private int              RoamDirection;

	public void Awake()
	{
		player           = GameObject.FindGameObjectWithTag("Player");
		playerController = player.GetComponent<PlayerController>();
		playerCollider   = player.GetComponent<CapsuleCollider2D>();
		mask   = LayerMask.GetMask("Player");

		State       = States.Roam;
		StartingPos = this.transform.position;

		RoamMaxTime = RoamDistanceMaximum / RoamingSpeed;
	}

	public void Update()
	{
		///////////////////////////////////////////////////////////////////////
		/// Checking if I am in the right state

		// Updating the distance to player
		DistanceToPlayer = Vector2.Distance(this.transform.position, player.transform.position);

		// Checking if I should start chasing the Player
		if ((DistanceToPlayer <= ChasePlayerRange) && (State != States.ChasePlayer))
		{
			// Casting a ray to check for line of sigh with player
			hit = Physics2D.Raycast(this.transform.position, player.transform.position - this.transform.position, ChasePlayerRange, mask);

			// Ray Cast to see if the player is visible to the Demon
			if (hit.collider.tag == "Player")
			{
				// Remebering that I am now chasing the player
				State = States.ChasePlayer;

				// Debug
				Debug.Log("Begin chasing the Player!!!");
			}
		}


		///////////////////////////////////////////////////////////////////////
		/// I have determined my state

		// If I am in the Roam State
		if (State == States.Roam)
		{
			// If I am ready for a new Roaming Direction
			if (ReadyForNewRoamDirection)
			{
				// The Direction the NPC will Roam in
				RoamDirection = Random.Range(0, 4 + 1); // <- +1 because int

				// The Distance the NPC will Roam
				RoamDistance = Random.Range(RoamDistanceMinimum, RoamDistanceMaximum);

				// The Roaming Position
				switch (RoamDirection)
				{
					// Up
					case (int)Direction.Up:
						RoamPosition = this.transform.position + new Vector3(0, RoamDistance, 0);
						break;
					// Right
					case (int)Direction.Right:
						RoamPosition = this.transform.position + new Vector3(RoamDistance, 0, 0);
						break;
					// Down
					case (int)Direction.Down:
						RoamPosition = this.transform.position + new Vector3(0, -RoamDistance, 0);
						break;
					// Left
					case (int)Direction.Left:
						RoamPosition = this.transform.position + new Vector3(-RoamDistance, 0, 0);
						break;
					// Stand Still
					default:
						RoamPosition = this.transform.position;
						break;
				}

				// Starting the Roaming Timer
				RoamingTimerInstance = RoamingTimer(RoamMaxTime);
				StartCoroutine(RoamingTimerInstance); // Incase the new Roaming Position is outside the bounds

				// Remeber that I no longer need a new Roaming Direction
				ReadyForNewRoamDirection = false;
			}

			// If I already have a Roaming Direction
			else
			{
				if (RoamPosition != null)
				{
					// Setting my max Step for this frame
					MaxRoamingStep = RoamingSpeed * Time.deltaTime;

					// Moving towards my target
					this.transform.position = Vector3.MoveTowards(this.transform.position, RoamPosition, MaxRoamingStep);
				}

				// If I have reached my target
				if (this.transform.position == RoamPosition)
				{
					// Remember that I need a new Roaming Direction
					ReadyForNewRoamDirection = true;

					// Haulting the Roaming Timer
					StopCoroutine(RoamingTimerInstance);
				}
			}
		}

		// If I am in the Chase Player State
		else if (State == States.ChasePlayer)
		{
			// Casting a ray to check for line of sigh with player
			hit = Physics2D.Raycast(this.transform.position, player.transform.position - this.transform.position, ChasePlayerRange, mask);

			// Checking if I should stop chasing the Player
			if ((DistanceToPlayer > ChasePlayerRange) || (hit.collider.tag != "Player"))
			{
				// Set my state back to Roam
				State = States.Roam;

				// Debug
				Debug.Log("Hault Chasing the Player");
			}
			
			// If I should be still chasing the player
			else
			{
				MaxChaseStep = ChasingSpeed * Time.deltaTime;
				
				this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, MaxChaseStep);
			}	
		}

	}
	
	// When I make first contact with 
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player")
        {
			DamagePlayerFrameDelayInstance = DamagePlayerFrameDelay();
			StartCoroutine(DamagePlayerFrameDelayInstance);
		}

		if ((col.gameObject.tag == "Boundary") && (State == States.Roam) && (ReadyForNewRoamDirection == false))
		{

			Debug.Log("Ran into the Wall");

			if ((RoamDirection == (int)Direction.Up) || (RoamDirection == (int)Direction.Down))
			{
				RoamPosition = new Vector3(1f * RoamPosition.x, -1f * RoamPosition.y, 1f * RoamPosition.z);

			}

			else if ((RoamDirection == (int)Direction.Left) || (RoamDirection == (int)Direction.Right))
			{
				RoamPosition = new Vector3(-1f * RoamPosition.x, 1f * RoamPosition.y, 1f * RoamPosition.z);

			}

			/*
			StopCoroutine(RoamingTimerInstance);
			ReadyForNewRoamDirection = true;
			*/
		}
	}
	
	void OnTriggerStay2D(Collider2D col)
	{
		if ((playerController.canTakeDamage == true) && (CanDamagePlayer == true))
		{
			StartCoroutine(playerController.PlayerHitAnimation());
			
			playerController.canTakeDamage = false;
			CanDamagePlayer = false;
			
			playerController.healthCurrent -= AttackDamage; 
			playerController.healthBar.SetHealth(playerController.healthCurrent);
			
			DamagePlayerFrameDelayInstance = DamagePlayerFrameDelay();
			StartCoroutine(DamagePlayerFrameDelayInstance);
		}
	}
	
	void OnTriggerExit2D(Collider2D col)
	{
		if (DamagePlayerFrameDelayInstance != null)
		{
			StopCoroutine(DamagePlayerFrameDelayInstance);
		}
	}
	
	void OnCollisionEnter2D(Collision2D col) {

		if (col.gameObject.tag == "Player")
		{
			Physics2D.IgnoreCollision(col.otherCollider, col.collider);
		}

	}
	
	public IEnumerator DamagePlayerFrameDelay()
	{
		yield return new WaitForSeconds(FramesToDamagePlayer); // <-- Change this to frames instead of seconds.
		
		CanDamagePlayer = true;
	}
	
	// Roaming Timer - Keeps me from Roaming for too long
	public IEnumerator RoamingTimer(float timer)
    {
		// Waits for my max roaming time
		yield return new WaitForSeconds(timer);

		// Remeber that I need a new Roaming Direction
		ReadyForNewRoamDirection = true;
    }

}
