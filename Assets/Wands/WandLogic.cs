using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandLogic : MonoBehaviour
{
	public Wand wandProp;
	private GameObject player, manaBar;
	private GameObject projectile;
	
	public bool playerIsCasting = false;
	public bool wandCanCast = true;
	
	private int spellIndex = 0;
	private float castSpreadValue = 0;
	private float projectileAngle;
	private Vector2 projectileDirection;

    private void Awake()
    {
		player = GameObject.FindGameObjectWithTag("Player");
		manaBar = GameObject.FindGameObjectWithTag("Mana Bar");
    }

    private void Update()
    {
		if(playerIsCasting && wandCanCast)
		{
			CastSpell(wandProp, spellIndex);


			if (spellIndex < wandProp.Spells.Count - 1)
			{
				spellIndex += 1;
			}
			else 
			{
				spellIndex = 0;
			}
		}
	}
	
	public void CastSpell(Wand wandProp, int spellIndex)
	{
		// Create the projectile
		projectile = (GameObject)Instantiate(wandProp.Spells[spellIndex].spellPreFab);
		
		// Tell the projectile to ignore collisions with the player
		Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), player.transform.gameObject.GetComponent<CapsuleCollider2D>());
		
		// Finding the projectile's direction based on cursor Position and Cast Spread
		projectileDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - this.transform.position;
		projectileAngle = Mathf.Atan2(projectileDirection.y, projectileDirection.x);
		
		castSpreadValue = Mathf.Deg2Rad * Random.Range(-wandProp.castSpread, wandProp.castSpread);

		projectileAngle += castSpreadValue;
		projectileDirection = new Vector2(Mathf.Cos(projectileAngle), Mathf.Sin(projectileAngle));
		
		// Setting the projectile's velocity and target
		projectile.transform.gameObject.GetComponent<ProjectileSpell>().projectileVelocity = wandProp.Spells[spellIndex].speed * (projectileDirection).normalized;
		projectile.transform.gameObject.GetComponent<ProjectileSpell>().reticlePosRelativeToHilt = projectileDirection;


		player.transform.gameObject.GetComponent<PlayerController>().manaCurrent -= wandProp.Spells[spellIndex].manaCost;
		manaBar.transform.gameObject.GetComponent<ManaBarScript>().SetMana(player.transform.gameObject.GetComponent<PlayerController>().manaCurrent);
		
		wandCanCast = false;
		StartCoroutine(castTimer(wandProp.Spells[spellIndex].castDelay + wandProp.castDelay));

	}
	
	IEnumerator castTimer(float waitTime)
    {
		if(waitTime > 0)
		{
			yield return new WaitForSeconds(waitTime);
			wandCanCast = true;
		}
		else
		{
			wandCanCast = true;
		}
        
    }
	
	public static Vector3 getMouseWorldPosition()
    {
        Camera worldCamera = Camera.main;
        Vector3 vec = worldCamera.ScreenToWorldPoint(Input.mousePosition);

        vec.z = 0f;
        return vec;
    }

}