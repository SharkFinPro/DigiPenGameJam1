using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public float Speed = 3f;
    
    //Rotation Speed
    public float SpeedRotation = 100f;

    private int wanderTrue = 0;
    /*
    private int rotatingRightTrue = 0;
    private int rotatingLeftTrue = 0;
    private int movement = 0; */

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (wanderTrue == 0)
        {
            StartCoroutine(Wander());
        }
        /*if(rotatingRightTrue == 1)
        {
            transform.Rotate(transform.up * Time.deltaTime * SpeedRotation);
        }
        if (rotatingLeftTrue == 1)
        {
            transform.Rotate(transform.up * Time.deltaTime * -SpeedRotation);
        }*/
        if (wanderTrue == 1)
        {
            transform.position += (transform.forward * Time.deltaTime * Speed);
        }
    }

    IEnumerator Wander()
    {
        //Movement
        int BreakWalk = 4; //Random.Range(1, 4);
        int MoveTime = 4; //Random.Range(1, 5);

        //Rotation
        /*
        int timeRotation = Random.RandomRange(1, 3);
        int BreakRotate = Random.Range(1, 4);
        int rotateDir = Random.Range(1, 2); */

        //Set Wandering to true and start wandering
        wanderTrue = 1;

        //Timer code I found on the internet : (yield return new WaitForSeconds(BreakWalk);) 
        //waits for random seconds to pass then after sets to false

        //Waiting certain time to walk and to stop
        yield return new WaitForSeconds(BreakWalk);
        wanderTrue = 1;
        yield return new WaitForSeconds(BreakWalk);
        wanderTrue = 0;

        //Same for ropatation
        //Rotating Right
        /*
        yield return new WaitForSeconds(BreakRotate);
        if (rotateDir == 1)
        {
            rotatingRightTrue = 1;
            yield return new WaitForSeconds(timeRotation);
            rotatingRightTrue = 0;
        }
        if (rotateDir == 2)
        {
            rotatingLeftTrue = 1;
            yield return new WaitForSeconds(timeRotation);
            rotatingLeftTrue = 0;
        }
        wanderTrue = 0; */
    }
}
