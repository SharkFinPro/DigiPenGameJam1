////////////////
///By: Flynn Duniho
///Date: 11/15/20
///Description: Tints sprites a random color
////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.HSVToRGB(Random.Range(0f,1f), 1, 1);
    }
}
