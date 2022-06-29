using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{

	[SerializeField]
	private float activeTime = 0.1f;
	[SerializeField]
	private float alphaSet = 0.8f;
	private float timeActivated;
	private float alpha;
	private float alphaMultiplier = 0.85f;

	private Transform player;

	private SpriteRenderer SR;
	private SpriteRenderer playerSR;

	private Color colour;

	private void OnEnable()
	{
		SR = GetComponent<SpriteRenderer>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerSR = player.GetComponent<SpriteRenderer>();

		alpha = alphaSet;
		SR.sprite = playerSR.sprite;
		transform.position = player.position;
		transform.rotation = player.rotation;
		timeActivated = Time.time;
	}

	private void Update()
	{
		alpha *= alphaMultiplier;
		colour = new Color(1f, 1f, 1f, alpha);
		SR.color = colour;

		if (Time.time >= (timeActivated + activeTime))
		{
			PlayerAfterImagePool.Instance.AddToPool(gameObject);
		}
	}
}