﻿/**************************
 * File: CameraController
 * Author: Flynn Duniho
 * Description: Follow the player, with lerp. Supports bounds, currently not used
**************************/
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("GameObject to follow")]
    public GameObject Follow;

    [Tooltip("Larger values make the camera slower")]
    public float Smoothness = 2;

    [Tooltip("Limits of camera movement. Can be null")]
    public Collider2D Bounds;



    //Camera view width and height
    private float w, h;

    //Camera component
    private Camera cam;

    //offset for camera shake
    private Vector3 offset;

    //Counter for camera shake
    private Stopwatch shake;

    //Camera shake magnitude
    private Vector3 shakeAmount;

    //Camera shake duration
    private long shakeDuration;


    void Start()
    {
        shakeAmount = Vector3.zero;
        offset = Vector3.zero;
        shake = new Stopwatch();

        //Get width and height
        cam = GetComponent<Camera>();
        h = cam.orthographicSize * 2;
        w = h * cam.aspect;
    }

    void FixedUpdate()
    {
        if (shake.IsRunning)
        {
            //Randomize offset for camera shake
            offset = new Vector3(Random.Range(-shakeAmount.x, shakeAmount.x), Random.Range(-shakeAmount.y, shakeAmount.y), Random.Range(-shakeAmount.z, shakeAmount.z));

            //Camera shake duration has exceeded time limit
            if (shake.ElapsedMilliseconds > shakeDuration)
            {
                //Reset
                shake.Stop();
                offset = Vector3.zero;
            }
        }

        Vector3 newPos = Vector3.Lerp(transform.position, Follow.transform.position, 1 / Smoothness);
        newPos = new Vector3(newPos.x, newPos.y, -10); ;

        if (Bounds != null)
        {
            //Limit camera
            transform.position = new Vector3(Mathf.Clamp(newPos.x, Bounds.bounds.min.x + w / 2, Bounds.bounds.max.x - w / 2), Mathf.Clamp(newPos.y, Bounds.bounds.min.y + h / 2, Bounds.bounds.max.y - h / 2), newPos.z) + offset;
        }
        else
        {
            //No limit for camera
            transform.position = newPos + offset;
        }
    }

    /// <summary>
    /// Start shaking the camera. Anyone can call this method
    /// </summary>
    /// <param name="duration">Length of camera shake</param>
    /// <param name="amount">Maximum shake movement</param>
    public void Shake(float duration, Vector3 amount)
    {
        shakeDuration = (long)(duration * 1000);
        shakeAmount = amount;
        shake.Restart();
    }
}
