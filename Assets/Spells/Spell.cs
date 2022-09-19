using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
	public enum SpellType
    {
		Proejctile,
		Modifier
    }

	public GameObject spellPreFab;
	public SpellType TypeOfSpell;

	public int manaCost;
	public int damage;

	public float castDelay;
	public float speed;

	public Vector2 velocity;
}