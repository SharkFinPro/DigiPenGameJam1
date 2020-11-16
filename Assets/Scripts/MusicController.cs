////////////////
///By: Flynn Duniho
///Date: 11/16/20
///Description: Controls music
////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController Music;
    // Start is called before the first frame update
    void Start()
    {
        if (Music == null)
        {
            Music = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
