using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpell : MonoBehaviour
{
    public Rigidbody2D RB;
    public GameObject Weapon;

    public Vector2 reticlePosRelativeToHilt;
    public Vector2 projectileVelocity;

    private void FixedUpdate()
    {
        RB.velocity = projectileVelocity;
        this.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(reticlePosRelativeToHilt.y, reticlePosRelativeToHilt.x));
    }

    private void Awake()
    {
        // Grabbing 
        Weapon = GameObject.FindGameObjectWithTag("Weapon");

        this.transform.SetParent(Weapon.transform);
        this.transform.localPosition = new Vector2(0f, 0f);

        Debug.Log(projectileVelocity);

        this.transform.SetParent(null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
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