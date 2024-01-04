using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    protected GameManager Manager;

    protected GameState(GameManager manager)
    {
        Manager = manager;
    }

    public abstract void OnEnter();
    public abstract void OnExit();
}
