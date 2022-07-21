using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandLogic : MonoBehaviour
{
	[SerializeField] private Collider2D playerBoundaryCollider;
	private GameObject projectile;
	private GameObject player;
	public ManaBarScript manaBar;
	public Wand wandProp;
	
	public bool playerIsCasting = false;
	public bool wandCanCast = true;
	
	private int spellIndex = 0;
	private float castSpreadValue = 0;
	private Vector2 projectileDirection;
	
	private void Update()
    {
		if(playerIsCasting && wandCanCast)
		{
			CastSpell(wandProp, spellIndex);
			
			if(spellIndex < wandProp.Spells.count)
			{
				spellIndex +=;
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
		projectile = (GameObject)Instantiate(wandProp.Spells[spellIndex].ProjectilePreFab);
		
		// Tell the projectile to ignore collisions with the player
		Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), playerBoundaryCollider);
		
		// Finding the projectile's direction based on cursor Position and Cast Spread
		projectileDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - this.transform.position;
		reticleAngle = Mathf.atan(projectileDirection.y, projectileDirection.x);
		
		castSpreadValue = Mathf.Deg2Rad * Random.Range(-wandProp.castSpread, wandProp.castSpread);
		
		reticleAngle += castSpreadValue;
		projectileDirection = new Vector2(Mathf.cos(reticleAngle), Mathf.sin(reticleAngle));
		
		// Setting the projectile's velocity
		projectile.Veloicty = wandProp.Spells[spellIndex].speed * (projectileDirection).normalized;
		
		player.manaCurrent -= wandProp.Spells[spellIndex].manaCost;
		manaBar.SetMana(player.manaCurrent);
		
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