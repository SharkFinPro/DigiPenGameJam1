using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float Speed = 2f;
    private int direction;

    public Transform Position1;
    public Transform Position2;
    public Transform StartPosition;
    private Vector3 NextPos;

    // Start is called before the first frame update
    void Start()
    {
        NextPos = StartPosition.position;
        //direction = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == Position1.position)
        {
            NextPos = Position2.position;
            //direction = 0;
        }
        else if (transform.position == Position2.position)
        {
            NextPos = Position1.position;
            //direction = 1;
        }

        /*if (direction == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, Position1.position, Speed * Time.deltaTime);
        }
        else if (direction == 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, Position2.position, Speed * Time.deltaTime);
        }*/
        transform.position = Vector3.MoveTowards(transform.position, NextPos, Speed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Position1.position, Position2.position);
    }
}
