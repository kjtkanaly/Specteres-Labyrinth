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
	private float RoamDistanceMinimum = 1f;
	private float RoamDistanceMaximum = 4f;
	private float chasePlayerRange = 15f;
	private bool  ReadyForNewRoamDirection = true;

	private GameObject   player;
	private RaycastHit2D hit;
	private LayerMask    mask;
	private IEnumerator  RoamingTimerInstance;
	public  States       State;
	public  Vector3      StartingPos;
	public  Vector3      RoamPosition;
	private float        RoamMaxTime;
	private float        maxRoamingStep;
	private float        RoamDistance;
	public  float        distanceToPlayer;
	private int          RoamDirection;

	public void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
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
		distanceToPlayer = Vector2.Distance(this.transform.position, player.transform.position);

		// Checking if I should start chasing the Player
		if ((distanceToPlayer <= chasePlayerRange) && (State != States.ChasePlayer))
		{
			// Casting a ray to check for line of sigh with player
			hit = Physics2D.Raycast(this.transform.position, player.transform.position - this.transform.position, chasePlayerRange, mask);

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
					case 1:
						RoamPosition = this.transform.position + new Vector3(0, RoamDistance, 0);
						break;
					// Right
					case 2:
						RoamPosition = this.transform.position + new Vector3(RoamDistance, 0, 0);
						break;
					// Down
					case 3:
						RoamPosition = this.transform.position + new Vector3(0, -RoamDistance, 0);
						break;
					// Left
					case 4:
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
					maxRoamingStep = RoamingSpeed * Time.deltaTime;

					// Moving towards my target
					this.transform.position = Vector3.MoveTowards(this.transform.position, RoamPosition, maxRoamingStep);
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
			hit = Physics2D.Raycast(this.transform.position, player.transform.position - this.transform.position, chasePlayerRange, mask);

			// Checking if I should stop chasing the Player
			if ((distanceToPlayer > chasePlayerRange) || (hit.collider.tag != "Player"))
			{
				// Set my state back to Roam
				State = States.Roam;

				// Debug
				Debug.Log("Hault Chasing the Player");
			}
		}

	}
	
	
	void OnCollisionEnter2D(Collision2D col)
	{
		Debug.Log(col.GameObject.name);
		
		if ((col.GameObject.tag == "Boundary") && (state == States.Roam) && (ReadyForNewRoamDirection == false))
		{
			StopCoroutine(RoamingTimerInstance);
			ReadyForNewRoamDirection = true;
		}
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
