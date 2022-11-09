using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderState : State
{
    public OrderState(Agent owner, string name) : base(owner, name) { }

    public Vector3? destination;

    public override void OnEnter()
    {
        owner.movement.Resume();
        owner.movement.MoveTowards((Vector3)destination);
    }

    public override void OnUpdate()
    {
        if ((owner.transform.position - (Vector3)destination).magnitude <= 5)
        {
            owner.atDestination.value = true;
            owner.movement.Stop();
        }
    }

    public override void OnExit()
    {
        owner.movement.Stop();
    }
}
