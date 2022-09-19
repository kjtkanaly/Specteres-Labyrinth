using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandLogic : MonoBehaviour
{
	public  Wand wandProp;
	public  ObjectPooling SpellBank;
	private PlayerController PlayerControl;
	private ManaBarScript    ManaBar;
	
	public bool playerIsCasting = false;
	public bool wandCanCast = true;
	
	private int spellIndex = 0;
	private float castSpreadValue = 0;
	private float projectileAngle;
	public  Vector2 projectileDirection;

    private void Awake()
    {
		PlayerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		ManaBar       = GameObject.FindGameObjectWithTag("Mana Bar").GetComponent<ManaBarScript>();
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
		// Pull the Projectile from the Projectile Pool
		GameObject Spell = ObjectPooling.SharedInstance.GetPooledObject();

		if (Spell != null)
		{
			Spell.SetActive(true);

			// Setting the Projectile's Starting Pos
			Spell.transform.SetParent(this.transform);
			Spell.transform.localPosition = new Vector2(0f, 0f);

			// Finding the projectile's direction based on cursor Position and Cast Spread
			projectileDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - this.transform.position;
			projectileAngle = Mathf.Atan2(projectileDirection.y, projectileDirection.x);

			castSpreadValue = Mathf.Deg2Rad * Random.Range(-wandProp.castSpread, wandProp.castSpread);

			projectileAngle += castSpreadValue;
			projectileDirection = new Vector2(Mathf.Cos(projectileAngle), Mathf.Sin(projectileAngle));

			// Setting the Projectile's Angle
			Spell.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(projectileDirection.y, projectileDirection.x));

			// Setting the projectile's velocity using the pos of the reticle
			Spell.GetComponent<Rigidbody2D>().velocity = Spell.GetComponent<Spell>().speed * (projectileDirection).normalized;

			Spell.transform.SetParent(null);

			PlayerControl.manaCurrent -= wandProp.Spells[spellIndex].manaCost;
			ManaBar.SetMana(PlayerControl.manaCurrent);

			wandCanCast = false;
			StartCoroutine(castTimer(wandProp.Spells[spellIndex].castDelay + wandProp.castDelay));
		}

		else
		{
			Debug.Log("Spell Bank Empty... Sadge");
			return;
        }
	
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