using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NotifyPropertyChangedBase
{
    private static GameManager _manager;
    public static GameManager Manager
    {
        get
        {
            if(_manager == null)
            {
                _manager = new GameManager();
            }

            return _manager;
        }
    }

    private long _score = 0;
    public long Score
    {
        get { return _score; }
        set { UpdateField(ref _score, value); }
    }


    public GameManager ()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (_manager == null)
        {
            _manager = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
