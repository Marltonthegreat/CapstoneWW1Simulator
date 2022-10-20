using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public ChaseState(Agent owner, string name) : base(owner, name) { }

    public override void OnEnter()
    {
        owner.movement.Resume();
    }

    public override void OnUpdate()
    {
        owner.movement.MoveTowards(owner.enemy.transform.position);
    }

    public override void OnExit()
    {

    }
}
