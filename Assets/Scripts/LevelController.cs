/**
 * By Alex Martin
 * 11/8/2020
 * This is used to manage the levels
**/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private int level = 1;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(sceneName: "menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel()
    {
        level++;
        SceneManager.LoadScene(sceneName: level.ToString());
    }

    public void SetLevel(int lvl)
    {
        level = lvl;
        SceneManager.LoadScene(sceneName: level.ToString());
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(sceneName: level.ToString());
    }
}
