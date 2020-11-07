using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public GameObject Target;
    public float Interval;
    public float LaunchSpeed;

    private Stopwatch time;
    private bool launch;
    private bool cooldown;
    private PolygonCollider2D poly;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        time = new Stopwatch();
        poly = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (time.IsRunning)
        {
            if (!cooldown)
            {
                rb.rotation = -90 - Mathf.Rad2Deg * Mathf.Atan2(transform.position.x - Target.transform.position.x, transform.position.y - Target.transform.position.y);

                if (time.ElapsedMilliseconds > Interval * 1000)
                {
                    rb.velocity = transform.right * LaunchSpeed;
                    cooldown = true;
                    time.Restart();
                }
            }
            else
            {
                if (time.ElapsedMilliseconds > Interval * 1000)
                {
                    time.Stop();
                    cooldown = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == Target)
        {
            launch = true;

            if (!time.IsRunning)
                time.Restart();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == Target)
        {
            launch = false;
        }
    }
}
