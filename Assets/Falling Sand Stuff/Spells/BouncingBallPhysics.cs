using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBallPhysics : MonoBehaviour
{
    public Rigidbody2D BallRB;
    public Transform   BallTrans;

    public float Speed = 10f;

    public void Start()
    {
        BallRB    = this.GetComponent<Rigidbody2D>();
        BallTrans = this.GetComponent<Transform>();
    }

}
