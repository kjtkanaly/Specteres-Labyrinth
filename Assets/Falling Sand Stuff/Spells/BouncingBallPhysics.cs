using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBallPhysics : MonoBehaviour
{
    public Rigidbody2D BallRB;
    public Transform   BallTrans;
    public Transform   Parent;

    public Vector2 MouseDirection;

    public float Speed = 10f;

    public void Awake()
    {
        BallRB    = this.GetComponent<Rigidbody2D>();
        BallTrans = this.GetComponent<Transform>();        
    }

    public void Update()
    {
        if (BallRB.velocity.magnitude < 0.01f)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void OnActive()
    {
        this.transform.SetParent(Parent);

        this.transform.localPosition = new Vector3(0.8f, 0f);

        this.transform.SetParent(null);

        BallRB.velocity = MouseDirection.normalized * Speed;
        Debug.Log(BallRB.velocity);
    }
}
