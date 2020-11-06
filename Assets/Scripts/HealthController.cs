/**************************
 * File: HealthController
 * Author: Flynn Duniho
 * Description: Provides events for when health changes and when health reaches 0
**************************/
using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : NotifyPropertyChangedBase
{
    //Public health field, raises HealthChanged event when changed
    private int _health;
    public int Health
    {
        get { return _health; }
        set 
        { 
            UpdateField(ref _health, value);
            if (_health <= 0)
            {
                 Die?.Invoke();
            }
        }
    }


    [Tooltip("The starting health")]
    public int StartHealth = 3;

    public event Action Die;


    void Start()
    {
        _health = StartHealth;
    }
}
