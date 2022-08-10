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

	private IEnumerator RoamingTimerInstance;
	public  States      State;
	public  Vector3     StartingPos;
	public  Vector3     RoamPosition;
	private float       RoamMaxTime;
	private float       step;
	private float       RoamDistance;
	private int         RoamDirection;
	private bool        ReadyForNewRoamDirection = true;

	public void Awake()
	{
		State = States.Roam;
		StartingPos = this.transform.position;

		RoamMaxTime = RoamDistanceMaximum / RoamingSpeed;
	}

	public void Update()
	{
		if (State == States.Roam)
		{
			if (ReadyForNewRoamDirection)
			{
				// The Direction the NPC will Roam in
				RoamDirection = Random.Range(0, 4 + 1); // <- +1 because int

				// The Distance the NPC will Roam
				RoamDistance = Random.Range(RoamDistanceMinimum, RoamDistanceMaximum);

				// The Roaming Position
				switch (RoamDirection)
				{
					case 1:
						RoamPosition = this.transform.position + new Vector3(0, RoamDistance, 0);
						break;
					case 2:
						RoamPosition = this.transform.position + new Vector3(RoamDistance, 0, 0);
						break;
					case 3:
						RoamPosition = this.transform.position + new Vector3(0, -RoamDistance, 0);
						break;
					case 4:
						RoamPosition = this.transform.position + new Vector3(-RoamDistance, 0, 0);
						break;
					default:
						RoamPosition = this.transform.position;
						break;
				}

				RoamingTimerInstance = RoamingTimer(RoamMaxTime);
				StartCoroutine(RoamingTimerInstance); // Incase the new Roaming Position is outside the bounds
				ReadyForNewRoamDirection = false;
			}

			else
			{
				if (RoamPosition != null)
				{
					step = RoamingSpeed * Time.deltaTime;
					this.transform.position = Vector3.MoveTowards(this.transform.position, RoamPosition, step);
				}
				if (this.transform.position == RoamPosition)
				{
					ReadyForNewRoamDirection = true;
					StopCoroutine(RoamingTimerInstance);
				}
			}
		}
	}


	public IEnumerator RoamingTimer(float timer)
    {
		yield return new WaitForSeconds(timer);
    }



}
