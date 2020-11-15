using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public int level;
    public string PlayerTag;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag(PlayerTag))
        {
            SceneManager.LoadScene(level);
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
