/*
 *made by Abhi Kota
 * 11/14/2020
 * plays background creaking sound 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCreak : MonoBehaviour
{
    private AudioSource background;
    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!background.isPlaying)
        {
            background.Play();
        }
        
    }
}
