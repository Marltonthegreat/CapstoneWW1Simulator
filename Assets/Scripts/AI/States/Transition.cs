using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition
{
    Condition[] _conditions;

    public Transition(Condition[] conditions)
    {
        _conditions = conditions;
    }

    public bool ToTransition()
    {
        foreach (var condition in _conditions)
        {
            if (!condition.IsTrue()) return false;
        }

        return true;
    }
}
