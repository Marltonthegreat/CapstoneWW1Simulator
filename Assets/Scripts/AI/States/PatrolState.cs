using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public PatrolState(Agent owner, string name) : base(owner, name) { }

    public override void OnEnter()
    {
        if (owner.pathFollower.path != null)
        {
            owner.pathFollower.targetNode = owner.pathFollower.path.GetNearestNode(owner.transform.position);
            owner.movement.Resume();

            owner.timer.value = Random.Range(5, 10);
        }
        else
        {

        }
    }

    public override void OnUpdate()
    {
        owner.pathFollower.Move(owner.movement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            owner.stateMachine.SetState(owner.stateMachine.StateFromName(typeof(IdleState).Name));
        }
    }

    public override void OnExit()
    {
        owner.movement.Stop();
    }
}
