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
using System.Diagnostics;
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
    private bool canMove = true;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer render;
    private CapsuleCollider2D groundCol;
    private bool onGround;
    private bool prevOnGround = true;


    // Grappling
    private bool grappling = false;
    public float MaxGrapSpeed = 1.5f;
    public float GrapSpeedInc = 0.1f;
    public float FireDelay = 0.4f;
    public LayerMask IgnoreLayer;
    private float maxGrapLength = 150.0f;

    private LineRenderer grappleLine;
    public Material grappleMaterial;

    private Vector2 initGrap;
    private Vector2 grapLoc;
    private float grapPos;
    private float grapLength;

    private bool startGrapple;
    private Stopwatch fireDelay;
    //grapple sound
    private AudioSource grappleSound;
    public AudioClip grappleAct;
    public AudioClip grappleRetr;
   

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        groundCol = GetComponent<CapsuleCollider2D>();
        render = GetComponent<SpriteRenderer>();
        fireDelay = new Stopwatch();
        grappleSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // After grappling, have you tried to move again?
        if(!fireDelay.IsRunning && ((!canMove && Input.GetAxisRaw("Horizontal") != 0) || onGround))
        {
            canMove = true;
        }

        // Movement
        if (!grappling && canMove && !fireDelay.IsRunning)
        {
            DoMovement();
            if (Math.Abs(rb.velocity.x) > 0.1)
            {
                render.flipX = rb.velocity.x < 0;
            }
        }
        else if (grappling || fireDelay.IsRunning)
        {
            if (grappleLine != null)
            {
                render.flipX = rb.position.x > grappleLine.GetPosition(1).x;
            }
            else
            {
                render.flipX = rb.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            }
        }

        // Grapple
        if (Input.GetMouseButtonDown(0))
        {
            //Figure out firing angle range for animation
            Vector2 diff = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            float angle = Mathf.Asin(diff.y / diff.magnitude) * Mathf.Rad2Deg;
            if (angle < -45)
            {
                anim.SetInteger("FireAngle", -1);
            }
            else if (angle > 45)
            {
                anim.SetInteger("FireAngle", 1);
            }
            else
            {
                anim.SetInteger("FireAngle", 0);
            }

            startGrapple = true;
            anim.SetTrigger("Fire");
            anim.SetBool("Grappling", true);

            //Slight delay to play firing animation
            fireDelay.Restart();
            canMove = false;
            //rb.velocity = new Vector2(diff.x > 0 ? 0.15f : -0.15f, rb.velocity.y);
        }
        else if (Input.GetMouseButton(0) && grappling)
        {
            UpdateGrapple();
        }
        else if (!Input.GetMouseButton(0) && grappling)
        {
            DestroyGrapple();
        }

        if (startGrapple && fireDelay.ElapsedMilliseconds > FireDelay * 1000f)
        {
            fireDelay.Stop();
            startGrapple = false;
            StartGrappling();
        }

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

        if (!onGround)
        {
            if (rb.velocity.y > 0)
            {
                anim.SetBool("Grappling", true);
                anim.SetBool("Falling", false);           
            }
            else
            {
                anim.SetBool("Falling", true);
                anim.SetBool("Grappling", false);
            }
        }
    }

    private void LateUpdate()
    {
        if (grappling && grappleLine != null)
            grappleLine.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f));
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

    private void DoMovement()
    {
        // movement
        float movement;
        float horizInput = Input.GetAxisRaw("Horizontal");
        // move left and right
        // if pressing left and right, use StartAccel
        if (Math.Abs(horizInput) >= float.Epsilon)
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

        // KEEP Y VELOCITY CONSISTENT!
        // this is why movement is best kept as just a float instead of a vector
        if (onGround)
        {
            rb.velocity = new Vector2(Mathf.Clamp(movement, -MaxSpeed, MaxSpeed), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(movement, rb.velocity.y);
        }
    }

    private void CreateGrapple()
    {
        grappleLine = new GameObject("Grapple").AddComponent<LineRenderer>();
        grappleLine.material = grappleMaterial;
        grappleLine.positionCount = 2;
        grappleLine.startWidth = 0.15f;
        grappleLine.useWorldSpace = true;
        grappleLine.numCapVertices = 50;
        grappleLine.material.color = Color.black;
    }

    private RaycastHit2D GetGrappleRaycast()
    {
        // Check if there is a wall to grapple to
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return Physics2D.Raycast(transform.position, (mousePos - (Vector2)transform.position).normalized, maxGrapLength, ~IgnoreLayer);
    }

    private void StartGrappling()
    {
        //onGround = false;
        //prevOnGround = false;
        RaycastHit2D hit = GetGrappleRaycast();

        if (hit)
        {
            // Create grappling line
            if (grappleLine == null)
            {
                CreateGrapple();
            }

            // Set grapple position
            grappleLine.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f));
            grappleLine.SetPosition(1, hit.point);
            //grappleLine.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            

            // Check if the grapple length is within bounds of the set max
            Vector2 p1 = grappleLine.GetPosition(0);
            Vector2 p2 = grappleLine.GetPosition(1);

            grapLength = (p2 - p1).magnitude;

            if (grapLength > maxGrapLength)
            {
                DestroyGrapple();
                return;
              
            }
            
            // Make sure gravity is not an issue, reset velocity
            //rb.bodyType = RigidbodyType2D.Kinematic;
            //rb.velocity = new Vector2(0, 0);
            
            //grapples sound 
            grappleSound.PlayOneShot(grappleAct,1);
           
            // Make sure the player knows it is grappling now
            grappling = true;
        }
    }

    private void UpdateGrapple()
    {
        // Grapple Movement
        //grapSpeed += Input.GetAxisRaw("Vertical") * grapSpeedInc;

        //if (grapSpeed > maxGrapSpeed)
        //{
        //    grapSpeed = maxGrapSpeed;
        //} else if (grapSpeed < -maxGrapSpeed)
        //{
        //    grapSpeed = -maxGrapSpeed;
        //}

        //grapSpeed /= 1.15f;

        //grapPos += grapSpeed;

        //// Stay in bounds of Grapple
        //if (grapPos < 0.0f)
        //{
        //    grapPos = 0.0f;
        //}
        //else if (grapPos > grapLength - blockClipDistance)
        //{
        //    grapPos = grapLength - blockClipDistance;
        //}

        //// Grapple movement calculation
        Vector2 p1 = grappleLine.GetPosition(0); // Point 1
        Vector2 p2 = grappleLine.GetPosition(1); // Point 2

        //float ydif = p2.y - p1.y;
        //float xdif = p2.x - p1.x;
        //float ang = (float)Math.Atan2(xdif, ydif);

        //float yd = (float)Math.Sin(ang) * grapPos;
        //float xd = (float)Math.Cos(ang) * grapPos;

        //// Move along the grappling line
        //grapLoc = new Vector2(p2.x < p1.x ? initGrap.x - xd : initGrap.x + xd, initGrap.y + yd);
        //rb.position = grapLoc;

        //grappleLine.SetPosition(0, grapLoc);
        rb.AddForce((p2 - p1) * GrapSpeedInc * Time.deltaTime);

        if(rb.velocity.magnitude > MaxGrapSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxGrapSpeed;
        }
    }

    private void DestroyGrapple()
    {
        anim.SetBool("Grappling", false);
        // Fling Player
        //rb.velocity = -(rb.position - (Vector2)grappleLine.GetPosition(1)).normalized * grapSpeed / Time.deltaTime;

        // Destroy Grapple
        Destroy(grappleLine.gameObject);
        grappling = false;

        // Regain normal physics
        //rb.bodyType = RigidbodyType2D.Dynamic;
        //grapple sound
        grappleSound.PlayOneShot(grappleRetr,0.5f);
        canMove = false;
    }
}
