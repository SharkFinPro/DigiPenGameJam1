﻿////////////////
///By: Orion Gordon
///Date: 11/5/20
///Description: Simple player controller for platformers
////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("How fast the player moves.")]
    public float Speed = 200;
    [Tooltip("How quickly the player accelerates.")]
    public float StartAccel = 2f;
    [Tooltip("How quickly the player decelerates.")]
    public float EndAccel = 10f;
    [Tooltip("MaximumSpeed")]
    public float MaxSpeed = 10f;

    private Rigidbody2D rb;
    private CapsuleCollider2D groundCol;
    private bool onGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCol = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // movement
        float movement;
        float horizInput = Input.GetAxisRaw("Horizontal");
        // move left and right
        // if pressing left and right, use StartAccel
        if(horizInput != 0)
        {
            // interpolate between current velocity and desired velocity
            movement = Mathf.Lerp(rb.velocity.x, horizInput * Speed * Time.deltaTime, StartAccel * Time.deltaTime);
        }
        // otherwise use EndAccel
        else
        {
            movement = Mathf.Lerp(rb.velocity.x, horizInput * Speed * Time.deltaTime, EndAccel * Time.deltaTime);
        }
        // KEEP Y VELOCITY CONSISTENT!
        // this is why movement is best kept as just a float instead of a vector
        rb.velocity = new Vector2(Mathf.Clamp(movement, -MaxSpeed, MaxSpeed), rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.IsTouching(groundCol))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.IsTouching(groundCol))
        {
            onGround = false;
        }
    }
}