using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTwo : MonoBehaviour
{
    public Rigidbody2D RB;

    public float gravityConstant    = -10f;
    public float mass               = 10f;
    public float terminalVelocity   = 100f;

    public Vector2 PlayerVelocity = new Vector2(0f,0f);

    public bool AddGravity = true;

    // Update is called once per frame
    void Update()
    {
        // Add gravity
        if (AddGravity)
        {
            PlayerVelocity += new Vector2(0f, gravityConstant * Time.deltaTime);
        }
        
        // Clamp Player Velocity
        PlayerVelocity = Vector2.ClampMagnitude(PlayerVelocity, terminalVelocity);

        // Update RB
        RB.velocity = PlayerVelocity;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Touchdown!");
        AddGravity = false;
        PlayerVelocity = new Vector2(PlayerVelocity.x, 0f);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        Debug.Log("Liftoff");
        AddGravity = true;
    }
}
