/**
 * By Alex Martin
 * 11/8/2020
 * This is used to manage the levels
**/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelController
{
    public static readonly int MaxLevel = 5;
    private static int _level = 0;
    public static int Level { get => _level; private set => _level = value; }

    public static void NextLevel()
    {
        Level++;
        if (Level >= MaxLevel)
        {
            Level = 0;
        }

        SceneManager.LoadScene(Level);
    }

    public static void SetLevel(int lvl)
    {
        Level = lvl;
        SceneManager.LoadScene(Level);
    }

    public static void ReloadLevel()
    {
        SceneManager.LoadScene(Level);
    }
}
