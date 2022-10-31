using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderState : State
{
    public OrderState(Agent owner, string name) : base(owner, name) { }

    public Vector3 destination;

    public override void OnEnter()
    {
        owner.movement.Resume();
        owner.movement.MoveTowards(destination);
    }

    public override void OnUpdate()
    {
        /*owner.pathFollower.Move(owner.movement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            owner.stateMachine.SetState(owner.stateMachine.StateFromName(typeof(IdleState).Name));
        }*/
    }

    public override void OnExit()
    {
        owner.movement.Stop();
    }

    public void SetDestination()
    {

    }
}
