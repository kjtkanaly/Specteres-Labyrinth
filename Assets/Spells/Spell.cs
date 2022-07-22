using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
	public GameObject spellPreFab;

	public int manaCost;
	public int damage;

	public float castDelay;
	public float speed;

	public Vector2 velocity;
}