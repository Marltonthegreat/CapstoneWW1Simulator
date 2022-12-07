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
        var position = owner.enemy.transform.position - (owner.attackDistance - .1f) * (owner.enemy.transform.position - owner.transform.position).normalized;
        owner.movement.MoveTowards(position);
    }

    public override void OnExit()
    {

    }
}
