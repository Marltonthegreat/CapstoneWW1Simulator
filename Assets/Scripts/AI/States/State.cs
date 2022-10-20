using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public Agent owner { get; private set; }
    public string name { get; private set; }

    public State(Agent owner, string name)
    {
        this.owner = owner;
        this.name = name;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
