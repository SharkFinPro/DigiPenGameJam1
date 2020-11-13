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
using System.Security.Cryptography;
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
    private Animator anim;
    private SpriteRenderer render;
    private CapsuleCollider2D groundCol;
    private bool onGround;
    private bool prevOnGround = true;


    // Grappling
    private bool grappling = false;
    public float maxGrapSpeed = 1.5f;
    private float grapSpeed = 0;
    public float grapSpeedInc = 0.1f;
    public LayerMask IgnoreLayer;
    private float maxGrapLength = 150.0f;
    private float blockClipDistance = 0.5f;

    private LineRenderer grappleLine;
    public Material grappleMaterial;

    private Vector2 initGrap;
    private Vector2 grapLoc;
    private float grapPos;
    private float grapLength;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        groundCol = GetComponent<CapsuleCollider2D>();
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // After grappling, have you tried to move again?
        if(!CanMove && Input.GetAxisRaw("Horizontal") != 0)
        {
            CanMove = true;
        }

        // Movement
        if (!grappling && CanMove)
        {
            doMovement();
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

    Vector2 getPosition()
    {
        return rb.position;
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

    private void doMovement()
    {
        if (onGround && !prevOnGround)
        {
            anim.SetTrigger("Land");
        }
        else
        {
            anim.ResetTrigger("Land");
            anim.SetBool("Falling", !onGround);
        }
        prevOnGround = onGround;

        // movement
        float movement;
        float horizInput = Input.GetAxisRaw("Horizontal");
        // move left and right
        // if pressing left and right, use StartAccel
        if (horizInput != 0)
        {
            // interpolate between current velocity and desired velocity
            movement = Mathf.Lerp(rb.velocity.x, horizInput * Speed * Time.deltaTime, StartAccel * Time.deltaTime);

            if(!anim.GetBool("Falling"))
                anim.SetBool("Walking", true);
        }
        // otherwise use EndAccel
        else
        {
            movement = Mathf.Lerp(rb.velocity.x, horizInput * Speed * Time.deltaTime, EndAccel * Time.deltaTime);

            if (Math.Abs(movement) < 0.1)
            {
                anim.SetBool("Walking", false);
            }
        }

        if (Math.Abs(movement) > 0.1)
        {
            render.flipX = movement < 0;
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

    private void createGrapple()
    {
        grappleLine = new GameObject("Grapple").AddComponent<LineRenderer>();
        grappleLine.material = grappleMaterial;
        grappleLine.positionCount = 2;
        grappleLine.startWidth = 0.15f;
        grappleLine.useWorldSpace = true;
        grappleLine.numCapVertices = 50;
        grappleLine.material.color = Color.black;
    }

    private RaycastHit2D getGrappleRaycast()
    {
        // Check if there is a wall to grapple to
        Vector2 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float pxdif = mousPos.x - getPosition().x;
        float pydif = mousPos.y - getPosition().y;

        return Physics2D.Raycast(initGrap, new Vector2(pxdif, pydif).normalized, maxGrapLength, ~IgnoreLayer);
    }

    private void startGrappling()
    {
        RaycastHit2D hit = getGrappleRaycast();

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
                destroyGrapple();
                return;
            }

            // Make sure gravity is not an issue, reset velocity
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = new Vector2(0, 0);

            // Make sure the player knows it is grappling now
            grappling = true;
        }
    }

    private void updateGrapple()
    {
        // Grapple Movement
        if (Input.GetKey(KeyCode.W))
        {
            grapSpeed += grapSpeedInc;
        }
        if (Input.GetKey(KeyCode.S))
        {
            grapSpeed -= grapSpeedInc;
        }

        if (grapSpeed > maxGrapSpeed)
        {
            grapSpeed = maxGrapSpeed;
        } else if (grapSpeed < -maxGrapSpeed)
        {
            grapSpeed = -maxGrapSpeed;
        }

        grapSpeed /= 1.15f;

        grapPos += grapSpeed;

        // Stay in bounds of Grapple
        if (grapPos < 0.0f)
        {
            grapPos = 0.0f;
        }
        else if (grapPos > grapLength - blockClipDistance)
        {
            grapPos = grapLength - blockClipDistance;
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

    private void destroyGrapple()
    {
        // Fling Player
        rb.velocity = -(rb.position - (Vector2)grappleLine.GetPosition(1)).normalized * grapSpeed / Time.deltaTime;

        // Destroy Grapple
        Destroy(grappleLine.gameObject);
        grappling = false;

        // Regain normal physics
        rb.bodyType = RigidbodyType2D.Dynamic;

        CanMove = false;
    }
}
