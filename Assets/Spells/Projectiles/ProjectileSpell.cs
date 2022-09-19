using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpell : MonoBehaviour
{
    public Rigidbody2D RB;
    public GameObject WandOfOrigin;
    public WandLogic WandControl;
    public Spell SpellProperties;

    public Vector2 reticlePosRelativeToHilt;
    public Vector2 projectileVelocity;

    private void Start()
    {
        // Get the Player Object
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Tell the projectile to ignore collisions with the player
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), player.transform.gameObject.GetComponent<CapsuleCollider2D>());
    }

    private void OnEnable()
    {


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false);
    }

    /*
    public static Vector3 getMouseWorldPosition()
    {
        Camera worldCamera = Camera.main;
        Vector3 vec = worldCamera.ScreenToWorldPoint(Input.mousePosition);

        vec.z = 0f;
        return vec;
    }
    */
}