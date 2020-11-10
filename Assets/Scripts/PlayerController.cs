////////////////
///By: Orion Gordon
///Date: 11/5/20
///Description: Simple player controller for platformers
////////////////

/*
 * Updated by Alex Martin
 * 11/10
 * Added Grappling
**/

using System;
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


    // Grappling
    private LineRenderer grappleLine;
    public Material grappleMaterial;
    private bool grappling = false;
    private Vector2 initGrap;
    private float grapMove;
    private float grapLength;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCol = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!grappling)
        {
            // movement
            float movement;
            float horizInput = Input.GetAxisRaw("Horizontal");
            // move left and right
            // if pressing left and right, use StartAccel
            if (horizInput != 0)
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
            if (onGround = true)
            {
                rb.velocity = new Vector2(Mathf.Clamp(movement, -MaxSpeed, MaxSpeed), rb.velocity.y);
            }
        }

        // Grapple
        if (Input.GetMouseButtonDown(0))
        {
            // Check if there is a wall to grapple to
            Vector2 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float pxdif = mousPos.x - getPosition().x;
            float pydif = mousPos.y - getPosition().y;

            RaycastHit2D hit = Physics2D.Raycast(initGrap +
                (new Vector2(pxdif, pydif) * groundCol.bounds.size.magnitude)
                , new Vector2(pxdif, pydif), 100.0f);

            if (hit)
            {
                // Create grappling line
                grappling = true;
                if (grappleLine == null)
                {
                    createGrapple();
                }

                grapMove = 0;
                initGrap = this.getPosition();
                grappleLine.SetPosition(0, initGrap);
                grappleLine.SetPosition(1, hit.point);

                Vector2 p1 = grappleLine.GetPosition(0);
                Vector2 p2 = grappleLine.GetPosition(1);

                float xdif = p2.x - p1.x;
                float ydif = p2.y - p1.y;
                grapLength = (float)Math.Sqrt(xdif * xdif + ydif * ydif);
            }

        }
        else if (Input.GetMouseButton(0) && grappling)
        {
            // Grapple Movement
            if (Input.GetKey(KeyCode.W))
            {
                grapMove += grapLength / 25;
            }
            if (Input.GetKey(KeyCode.S))
            {
                grapMove -= grapLength / 25;
            }
            // Stay in bounds of Grapple
            if (grapMove < 0.0f)
            {
                grapMove = 0.0f;
            }
            else if (grapMove > grapLength)
            {
                grapMove = grapLength;
            }

            // Grapple movement calculation
            Vector2 p1 = grappleLine.GetPosition(0); // Point 1
            Vector2 p2 = grappleLine.GetPosition(1); // Point 2

            float ydif = p2.y - p1.y;
            float ang = (float)Math.Asin(ydif / grapLength);

            float yd = (float)Math.Sin(ang) * grapMove;
            float xd = (float)Math.Cos(ang) * grapMove;

            // Move along the grappling line
            if (p2.x < p1.x)
            {
                rb.position = new Vector2(initGrap.x - xd, initGrap.y + yd);
            }
            else
            {
                rb.position = new Vector2(initGrap.x + xd, initGrap.y + yd);
            }
            
        }
        else if (Input.GetMouseButtonUp(0) && grappling)
        {
            Destroy(grappleLine);
            grappling = false;
        }
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

    Vector2 getPosition()
    {
        return rb.position;
    }

    void createGrapple()
    {
        grappleLine = new GameObject("Grapple").AddComponent<LineRenderer>();
        grappleLine.material = grappleMaterial;
        grappleLine.positionCount = 2;
        grappleLine.startWidth = 0.15f;
        grappleLine.useWorldSpace = true;
        grappleLine.numCapVertices = 50;
    }

}
