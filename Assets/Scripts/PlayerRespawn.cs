////////////////
///By: Dev Dhawan
///Date: 11/11/20
///Description: Respawns player and resets Level
////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public int level;
    public string PlayerTag;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag(PlayerTag))
        {
            LevelController.ReloadLevel();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}


