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
    bool CanMove = true;
    private Rigidbody2D rb;
    private CapsuleCollider2D groundCol;
    private bool onGround;


    // Grappling
    private bool grappling = false;
    private float grapSpeed = 0.25f;
    private float maxGrapLength = 10.0f;

    private LineRenderer grappleLine;
    public Material grappleMaterial;

    private Vector2 initGrap;
    private Vector2 grapLoc;
    private float grapPos;
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
        // after grappling, have you tried to move again?
        if(!CanMove && Input.GetAxisRaw("Horizontal") != 0)
        {
            CanMove = true;
        }
        if (!grappling && CanMove)
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
            if (onGround == true)
            {
                rb.velocity = new Vector2(Mathf.Clamp(movement, -MaxSpeed, MaxSpeed), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(movement, rb.velocity.y);
            }
        }

        // Grapple
        if (Input.GetMouseButtonDown(0))
        {
            startGrappling();
        }
        else if (Input.GetMouseButton(0) && grappling)
        {
            updateGrapple();
        }
        else if (Input.GetMouseButtonUp(0) && grappling)
        {
            destroyGrapple();
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

    void startGrappling()
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
            if (grappleLine == null)
            {
                createGrapple();
            }

            // Set grapple position
            grapPos = 0;
            initGrap = this.getPosition();
            grapLoc = initGrap;
            grappleLine.SetPosition(0, initGrap);
            grappleLine.SetPosition(1, hit.point);

            // Check if the grapple length is within bounds of the set max
            Vector2 p1 = grappleLine.GetPosition(0);
            Vector2 p2 = grappleLine.GetPosition(1);

            float xdif = p2.x - p1.x;
            float ydif = p2.y - p1.y;
            grapLength = (float)Math.Sqrt(xdif * xdif + ydif * ydif);

            if (grapLength > maxGrapLength)
            {
                Destroy(grappleLine);
                return;
            }

            // Make sure gravity is not an issue, reset velocity
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = new Vector2(0, 0);

            // Make sure the player knows it is grappling now
            grappling = true;
        }
    }

    void updateGrapple()
    {
        // Grapple Movement
        if (Input.GetKey(KeyCode.W))
        {
            grapPos += grapSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            grapPos -= grapSpeed;
        }

        // Stay in bounds of Grapple
        if (grapPos < 0.0f)
        {
            grapPos = 0.0f;
        }
        else if (grapPos > grapLength)
        {
            grapPos = grapLength;
        }

        // Grapple movement calculation
        Vector2 p1 = initGrap; // Point 1
        Vector2 p2 = grappleLine.GetPosition(1); // Point 2

        float ydif = p2.y - p1.y;
        float ang = (float)Math.Asin(ydif / grapLength);

        float yd = (float)Math.Sin(ang) * grapPos;
        float xd = (float)Math.Cos(ang) * grapPos;

        // Move along the grappling line
        grapLoc = new Vector2(p2.x < p1.x ? initGrap.x - xd : initGrap.x + xd, initGrap.y + yd);
        rb.position = grapLoc;

        grappleLine.SetPosition(0, grapLoc);
    }

    void destroyGrapple()
    {
        Destroy(grappleLine);
        grappling = false;

        // Regain normal physics
        rb.bodyType = RigidbodyType2D.Dynamic;

        CanMove = false;
    }
}
