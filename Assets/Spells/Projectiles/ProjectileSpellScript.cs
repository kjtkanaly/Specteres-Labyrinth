using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpellScript : MonoBehaviour
{
    public Rigidbody2D RB;
    public GameObject MC, Reticle, Weapon;
    private Animator Anime;

    private Vector2 reticlePosRelativeToHilt;
    public float bulletSpeed = 40f;
    public float bulletHitForce = 20f;


    private void Awake()
    {
        MC = GameObject.FindGameObjectWithTag("Player");
        Weapon = GameObject.FindGameObjectWithTag("Weapon");

        reticlePosRelativeToHilt = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Weapon.transform.position;

        this.transform.SetParent(Weapon.transform);
        this.transform.localPosition = new Vector2(0f, 0f);

        RB.velocity = bulletSpeed * (reticlePosRelativeToHilt).normalized;
        this.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(reticlePosRelativeToHilt.y, reticlePosRelativeToHilt.x));

        this.transform.SetParent(null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }

    public static Vector3 getMouseWorldPosition()
    {
        Camera worldCamera = Camera.main;
        Vector3 vec = worldCamera.ScreenToWorldPoint(Input.mousePosition);

        vec.z = 0f;
        return vec;
    }

}